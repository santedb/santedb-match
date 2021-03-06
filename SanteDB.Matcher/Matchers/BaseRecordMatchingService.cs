﻿/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2021-2-9
 */
using System.Collections.Generic;
using SanteDB.Core.Services;
using System.Reflection;
using System.Linq;
using SanteDB.Core.Model.Query;
using System;
using SanteDB.Core;
using SanteDB.Matcher.Configuration;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Interfaces;
using System.Linq.Expressions;
using SanteDB.Core.Attributes;
using SanteDB.Core.Diagnostics;
using SanteDB.Matcher.Exceptions;
using SanteDB.Core.Security;
using SanteDB.Matcher.Model;
using SanteDB.Core.Model.Serialization;
using SanteDB.Matcher.Definition;
using System.Collections;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Identified comparator
    /// </summary>
    public class IdentifiedComparator<T> : IEqualityComparer<T>
        where T : IdentifiedData
    {
        /// <summary>
        /// Determine equality
        /// </summary>
        public bool Equals(T x, T y)
        {
            return x.Key == y.Key;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        public int GetHashCode(T obj)
        {
            return obj.Key.GetHashCode();
        }
    }

    /// <summary>
    /// Represents base record matching service for SanteDB Matcher
    /// </summary>
    public abstract class BaseRecordMatchingService : IRecordMatchingService, IMatchReportFactory
    {
        // Tracer
        protected Tracer m_tracer = new Tracer("SanteDB.Matcher.Engine");

        /// <summary>
        /// Service name
        /// </summary>
        public abstract string ServiceName { get; }

        /// <summary>
        /// Static CTOR
        /// </summary>
        static BaseRecordMatchingService()
        {
            foreach (var t in typeof(BaseRecordMatchingService).Assembly.ExportedTypes.Where(t => typeof(IQueryFilterExtension).IsAssignableFrom(t) && !t.IsAbstract))
                QueryFilterExtensions.AddExtendedFilter(Activator.CreateInstance(t) as IQueryFilterExtension);
            ModelSerializationBinder.RegisterModelType(typeof(MatchConfiguration));
        }

        /// <summary>
        /// Perform the record matching services
        /// </summary>
        /// <typeparam name="T">The type of object to be classified</typeparam>
        /// <param name="input">The input to be blocked</param>
        /// <param name="ignoreKeys">The keys to ignore for blocking</param>
        /// <param name="configurationName">The name of the configuration to use</param>
        /// <returns>The blocked records</returns>
        public virtual IEnumerable<T> Block<T>(T input, string configurationName, IEnumerable<Guid> ignoreKeys) where T : IdentifiedData
        {
            this.m_tracer.TraceVerbose("Will block {0} on configuration {1}", input, configurationName);

            try
            {
                if (EqualityComparer<T>.Default.Equals(default(T), input)) throw new ArgumentNullException(nameof(input), "Input classifier is required");

                // Get configuration if specified
                var configService = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>();
                if (configService == null)
                    throw new InvalidOperationException("Cannot find configuration service for matching");
                var config = configService.GetConfiguration(configurationName);
                if (config == null)
                    throw new KeyNotFoundException($"Cannot find configuration named {configurationName}");
                config = (config as MatchConfigurationCollection)?.Configurations.FirstOrDefault(o => o.Target.Any(t => typeof(T).IsAssignableFrom(t.ResourceType) || input.GetType().IsAssignableFrom(t.ResourceType))) ?? config;
                if (config == null || !(config is MatchConfiguration))
                    throw new InvalidOperationException($"Configuration {config?.GetType().Name ?? "null"} is not compatible with this match provider or is not registered");

                var strongConfig = config as MatchConfiguration;
                if (!strongConfig.Target.Any(t => t.ResourceType.IsAssignableFrom(input.GetType()) || t.ResourceType.IsAssignableFrom(typeof(T))))
                    throw new InvalidOperationException($"Configuration {strongConfig.Name} doesn't appear to contain any reference to {typeof(T).FullName}");

                // If the blocking algorithm for this type is AND then we can just use a single IMSI query
                if (strongConfig.Blocking.Count > 0)
                {
                    IEnumerable<T> retVal = null;
                    foreach (var b in strongConfig.Blocking)
                        if (retVal == null)
                            retVal = this.DoBlock<T>(input, b, ignoreKeys);
                        else if (b.Operator == BinaryOperatorType.AndAlso)
                        {
#if DEBUG
                            this.m_tracer.TraceVerbose("INTERSECT {0} blocked records against filter {1}", retVal.Count(), b.Filter);
#endif
                            retVal = retVal.Intersect(this.DoBlock<T>(input, b, ignoreKeys), new IdentifiedComparator<T>());
#if DEBUG
                            this.m_tracer.TraceVerbose("INTERSECT against filter {0} resulted in {1} results", b.Filter, retVal.Count());
#endif
                        }
                        else if (b.Operator == BinaryOperatorType.OrElse)
                        {
#if DEBUG
                            this.m_tracer.TraceVerbose("UNION {0} blocked records against filter {1}", retVal.Count(), b.Filter);
#endif
                            retVal = retVal.Union(this.DoBlock<T>(input, b, ignoreKeys), new IdentifiedComparator<T>());
#if DEBUG
                            this.m_tracer.TraceVerbose("UNION against filter {0} resulted in {1} results", b.Filter, retVal.Count());
#endif
                        }
                    return retVal.ToList();
                }
                else
                    throw new InvalidOperationException($"Configuration {config.Name} contains no blocking instructions, cannot Block");
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error blocking input {0} - {1}", input, e.Message);
                throw new MatchingException($"Error blocking input {input}", e);
            }
        }

        /// <summary>
        /// Perform the block operation
        /// </summary>
        private IEnumerable<T> DoBlock<T>(T input, MatchBlock block, IEnumerable<Guid> ignoreKeys) where T : IdentifiedData
        {
            try
            {
                // Load the persistence service
                var persistenceService = ApplicationServiceContext.Current.GetService<IRepositoryService<T>>();
                if (persistenceService == null)
                    throw new InvalidOperationException($"Cannot find persistence service for {typeof(T).FullName}");

                // Perpare filter
                var filter = block.Filter;
                NameValueCollection qfilter = new NameValueCollection();
                foreach (var b in filter)
                {
                    var nvc = NameValueCollection.ParseQueryString(b);
                    foreach (var nv in nvc)
                        foreach (var val in nv.Value)
                            qfilter.Add(nv.Key, val);
                }

                if(ignoreKeys?.Any() == true)
                {
                    qfilter.Add("id", ignoreKeys.Select(o => $"!{o}"));
                }

                // Make LINQ query 
                var linq = QueryExpressionParser.BuildLinqExpression<T>(qfilter, new Dictionary<string, Func<Object>>()
                {
                    { "input", ((Func<T>)(() => input)) }
                }, safeNullable: true, lazyExpandVariables: false);

                // If the resolved query variables are all null we want to ignore the query
                qfilter = new NameValueCollection(QueryExpressionBuilder.BuildQuery(linq).ToArray());
                if (block.SkipIfNull && qfilter.All(o => o.Value.All(v => "null".Equals(v))))
                    return new List<T>();
                this.m_tracer.TraceVerbose("Will execute block query : {0}", linq);
                // Total results
                int tr = 0;

                // Set the authentication context 
                using (AuthenticationContext.EnterSystemContext()) { 
                    var retVal = persistenceService.Find(linq, 0, block.MaxReuslts, out tr);

                    //                var retVal = persistenceService.Query(linq, 0, block.MaxReuslts, out tr, AuthenticationContext.SystemPrincipal);

                    if (tr > block.MaxReuslts)
                    {
                        this.m_tracer.TraceWarning($"Block condition {linq} results {tr} exceeds configured maximum of {block.MaxReuslts}!! Records will be ignored for consideration!");
                    }

                    return retVal;

                }
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Could not execute block {0} against {1} on storage provider: {2}", String.Join(" / ", block.Filter), input, e.Message);
                throw new MatchingException($"Could not execute block {String.Join(" / ", block.Filter)} against {input}", e);
            }
        }

        /// <summary>
        /// Classiries the specified blocks into matching results
        /// </summary>
        public abstract IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName) where T : IdentifiedData;

        /// <summary>
        /// Performs a block and match operation in one call
        /// </summary>
        public abstract IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName, IEnumerable<Guid> ignoreKeys) where T : IdentifiedData;

        /// <summary>
        /// Create a match report from the record type
        /// </summary>
        public object CreateMatchReport(Type recordType, object input, IEnumerable<IRecordMatchResult> matches)
        {
            return new MatchReport()
            {
                Input = (input as IdentifiedData)?.Key.Value ?? Guid.Empty,
                Results = matches.Select(o => new MatchResultReport(new MatchResult<IdentifiedData>(o.Record, o.Score, o.Strength, o.Classification, o.Method, o.Vectors.OfType<MatchVector>()))).ToList()
            };
        }

        /// <summary>
        /// Create a match report 
        /// </summary>
        public object CreateMatchReport<T>(T input, IEnumerable<IRecordMatchResult<T>> matches) where T : IdentifiedData
        {
            return this.CreateMatchReport(typeof(T), input, matches.OfType<IRecordMatchResult>());
        }

        /// <summary>
        /// Perform the specified match on <paramref name="input"/>
        /// </summary>
        public IEnumerable<IRecordMatchResult> Match(IdentifiedData input, string configurationName, IEnumerable<Guid> ignoreKeys)
        {
            // TODO: Provide a lookup list with a lambda expression to make this go faster
            var genMethod = typeof(BaseRecordMatchingService).GetGenericMethod(nameof(Match), new Type[] { input.GetType() }, new Type[] { input.GetType(), typeof(String), typeof(IEnumerable<Guid>) });
            var results = genMethod.Invoke(this, new object[] { input, configurationName, ignoreKeys }) as IEnumerable;
            return results.OfType<IRecordMatchResult>();
        }
    }
}