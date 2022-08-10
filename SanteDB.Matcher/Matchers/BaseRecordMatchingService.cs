﻿/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2021-8-27
 */
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Matching;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.Interfaces;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Model.Serialization;
using SanteDB.Core.Security;
using SanteDB.Core.Services;
using SanteDB.Matcher.Definition;
using SanteDB.Matcher.Diagnostics;
using SanteDB.Matcher.Exceptions;
using SanteDB.Matcher.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        /// <summary>
        /// Trace logger
        /// </summary>
        protected readonly Tracer m_tracer = new Tracer("SanteDB.Matcher.Engine");

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
        /// <param name="collector">The collector to use for diagnostics</param>
        /// <returns>The blocked records</returns>
        public virtual IQueryResultSet<T> Block<T>(T input, string configurationName, IEnumerable<Guid> ignoreKeys, IRecordMatchingDiagnosticSession collector = null) where T : IdentifiedData
        {
            this.m_tracer.TraceVerbose("Will block {0} on configuration {1}", input, configurationName);

            try
            {
                collector?.LogStartStage("blocking");
                if (EqualityComparer<T>.Default.Equals(default(T), input)) throw new ArgumentNullException(nameof(input), "Input classifier is required");

                // Get configuration if specified
                var configService = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>();
                if (configService == null)
                    throw new InvalidOperationException("Cannot find configuration service for matching");
                var config = configService.GetConfiguration(configurationName);
                if (config == null)
                    throw new KeyNotFoundException($"Cannot find configuration named {configurationName}");

                if (config is MatchConfigurationCollection collection)
                {
                    config = collection.Configurations.FirstOrDefault(o => o.Target.Any(t => typeof(T).IsAssignableFrom(t.ResourceType) || input.GetType().IsAssignableFrom(t.ResourceType))) ?? config;
                }

                if (config == null || !(config is MatchConfiguration strongConfig))
                    throw new InvalidOperationException($"Configuration {config?.GetType().Name ?? "null"} is not compatible with this provider");

                if (!strongConfig.Target.Any(t => t.ResourceType.IsAssignableFrom(input.GetType()) || t.ResourceType.IsAssignableFrom(typeof(T))))
                    throw new InvalidOperationException($"Configuration {strongConfig.Id} doesn't appear to contain any reference to {typeof(T).FullName}");

                // if there is no blocking config, we need to throw an error
                if (!strongConfig.Blocking.Any())
                {
                    throw new InvalidOperationException($"Configuration {config.Id} contains no blocking instructions, cannot Block");
                }

                IQueryResultSet<T> retVal = null;

                foreach (var b in strongConfig.Blocking)
                {
                    if (retVal == null)
                        retVal = this.DoBlock<T>(input, b, ignoreKeys, collector);
                    else if (b.Operator == BinaryOperatorType.AndAlso)
                    {
#if DEBUG
                        this.m_tracer.TraceVerbose("INTERSECT blocked records against filter {1}", retVal.Count(), b.Filter);
#endif

                        retVal = retVal.Intersect(this.DoBlock<T>(input, b, ignoreKeys, collector));

#if DEBUG
                        this.m_tracer.TraceVerbose("INTERSECT against filter {0} resulted in {1} results", b.Filter, retVal.Count());
#endif
                    }
                    else if (b.Operator == BinaryOperatorType.OrElse)
                    {
#if DEBUG
                        this.m_tracer.TraceVerbose("UNION {0} blocked records against filter {1}", retVal.Count(), b.Filter);
#endif

                        retVal = retVal.Union(this.DoBlock<T>(input, b, ignoreKeys, collector));

#if DEBUG
                        this.m_tracer.TraceVerbose("UNION against filter {0} resulted in {1} results", b.Filter, retVal.Count());
#endif
                    }
                }

                if (ignoreKeys != null)
                {
                    return retVal.Where(r => !ignoreKeys.Contains(r.Key.Value) && r.Key.Value != input.Key.Value);
                }
                else
                {
                    return retVal;
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error blocking input {0} - {1}", input, e.Message);
                throw new MatchingException($"Error blocking input {input}", e);
            }
            finally
            {
                collector?.LogEndStage();
            }
        }

        /// <summary>
        /// Perform the block operation
        /// </summary>
        private IQueryResultSet<T> DoBlock<T>(T input, MatchBlock block, IEnumerable<Guid> ignoreKeys, IRecordMatchingDiagnosticSession collector = null) where T : IdentifiedData
        {
            // Load the persistence service
            try
            {
                collector?.LogStartAction(block);
                // Perpare filter
                var filter = block.Filter;
                NameValueCollection qfilter = new NameValueCollection();
                foreach (var b in filter)
                {
                    bool shouldIncludeExpression = true;
                    if (b.When?.Any() == true) // Only include the filter when the conditions are met
                    {
                        var guardExpression = b.GuardExpression;

                        if (guardExpression == null) // not built
                        {
                            var parameter = Expression.Parameter(typeof(T));
                            Expression guardBody = null;
                            foreach (var whenClause in b.When)
                            {
                                var selectorExpression = QueryExpressionParser.BuildPropertySelector<T>(whenClause, true);
                                if (guardBody == null)
                                {
                                    guardBody = Expression.MakeBinary(ExpressionType.NotEqual, Expression.Invoke(selectorExpression, parameter), Expression.Constant(null));
                                }
                                else
                                {
                                    guardBody = Expression.MakeBinary(ExpressionType.AndAlso, Expression.Invoke(guardBody, parameter), Expression.MakeBinary(ExpressionType.NotEqual, Expression.Invoke(selectorExpression, parameter), Expression.Constant(null)));
                                }
                            }
                            b.GuardExpression = guardExpression = Expression.Lambda(guardBody, parameter).Compile();
                        }

                        shouldIncludeExpression = (bool)guardExpression.DynamicInvoke(input);
                    }

                    if (shouldIncludeExpression)
                    {
                        var nvc = NameValueCollection.ParseQueryString(b.Expression);
                        // Build the expression
                        foreach (var nv in nvc)
                        {
                            foreach (var val in nv.Value)
                            {
                                qfilter.Add(nv.Key, val);

                            }
                        }
                    }
                }

                // Do we skip when no conditions?
                if (!qfilter.Any())
                {
                    return new MemoryQueryResultSet<T>(new List<T>());
                }

                // Add ignore clauses
                if (ignoreKeys?.Any() == true)
                {
                    qfilter.Add("id", ignoreKeys.Select(o => $"!{o}"));
                }
                if (input.Key.HasValue)
                {
                    qfilter.Add("id", $"!{input.Key}");
                }

                // Make LINQ query
                // NOTE: We can't build and store this since input is a closure
                // TODO: Figure out a way to compile this expression once
                var linq = QueryExpressionParser.BuildLinqExpression<T>(qfilter, new Dictionary<string, Func<Object>>()
                {
                    { "input", ((Func<T>)(() => input)) }
                }, safeNullable: true, lazyExpandVariables: false);

                // Add status keys
                if (typeof(IHasState).IsAssignableFrom(typeof(T)))
                {
                    var stateAccess = Expression.MakeMemberAccess(linq.Parameters[0], typeof(T).GetProperty(nameof(IHasState.StatusConceptKey)));
                    Expression statePart = null;
                    foreach (var stateKey in StatusKeys.ActiveStates)
                    {

                        if(statePart == null)
                        {
                            statePart = Expression.MakeBinary(ExpressionType.Equal, stateAccess, Expression.Convert(Expression.Constant(stateKey), typeof(Guid?)));
                        }
                        else
                        {
                            statePart = Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(ExpressionType.Equal, stateAccess, Expression.Convert(Expression.Constant(stateKey), typeof(Guid?))), statePart);
                        }
                    }
                    linq = Expression.Lambda<Func<T, bool>>(Expression.MakeBinary(ExpressionType.AndAlso,
                            statePart, linq.Body), linq.Parameters[0]);
                }
                this.m_tracer.TraceVerbose("Will execute block query : {0}", linq);

                // Query control variables for iterating result sets

                // Set the authentication context
                using (AuthenticationContext.EnterSystemContext())
                {
                    IQueryResultSet<T> results = null;

                        if (block.UseRawPersistenceLayer)
                        {
                            var persistenceService = ApplicationServiceContext.Current.GetService<IDataPersistenceService<T>>();
                            if (persistenceService == null)
                                throw new InvalidOperationException($"Cannot find persistence service for {typeof(T).FullName}");

                            results = persistenceService.Query(linq, AuthenticationContext.SystemPrincipal);
                            
                        }
                        else
                        {
                            var persistenceService = ApplicationServiceContext.Current.GetService<IRepositoryService<T>>();
                            if (persistenceService == null)
                                throw new InvalidOperationException($"Cannot find persistence service for {typeof(T).FullName}");

                            results = persistenceService.Find(linq);
                            
                    }

                    collector?.LogSample(linq.ToString(), results.Count());
                    return results;
                }
            }
            finally
            {
                collector?.LogEndAction();
            }
        }

        /// <summary>
        /// Classiries the specified blocks into matching results
        /// </summary>
        public abstract IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName, IRecordMatchingDiagnosticSession collector = null) where T : IdentifiedData;

        /// <summary>
        /// Performs a block and match operation in one call
        /// </summary>
        public abstract IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName, IEnumerable<Guid> ignoreKeys, IRecordMatchingDiagnosticSession collector = null) where T : IdentifiedData;

        /// <summary>
        /// Create a match report from the record type
        /// </summary>
        public object CreateMatchReport(Type recordType, object input, IEnumerable<IRecordMatchResult> matches, IRecordMatchingDiagnosticSession diagnosticSession = null)
        {
            return new MatchReport()
            {
                Diagnostics = diagnosticSession?.GetSessionData() as MatchDiagnostics,
                Results = matches.Select(o => new MatchResultReport(new MatchResult<IdentifiedData>(o.Record, o.Score, o.Strength, o.ConfigurationName, o.Classification, o.Method, o.Vectors))).ToList()
            };
        }

        /// <summary>
        /// Create a match report
        /// </summary>
        public object CreateMatchReport<T>(T input, IEnumerable<IRecordMatchResult<T>> matches, IRecordMatchingDiagnosticSession diagnosticSession = null) where T : IdentifiedData
        {
            return this.CreateMatchReport(typeof(T), input, matches.OfType<IRecordMatchResult>(), diagnosticSession);
        }

        /// <summary>
        /// Perform the specified match on <paramref name="input"/>
        /// </summary>
        public IEnumerable<IRecordMatchResult> Match(IdentifiedData input, string configurationName, IEnumerable<Guid> ignoreKeys, IRecordMatchingDiagnosticSession collector = null)
        {
            // TODO: Provide a lookup list with a lambda expression to make this go faster
            var genMethod = typeof(BaseRecordMatchingService).GetGenericMethod(nameof(Match), new Type[] { input.GetType() }, new Type[] { input.GetType(), typeof(String), typeof(IEnumerable<Guid>), typeof(IRecordMatchingDiagnosticSession) });
            var results = genMethod.Invoke(this, new object[] { input, configurationName, ignoreKeys, collector }) as IEnumerable;
            return results.OfType<IRecordMatchResult>();
        }

        /// <summary>
        /// Classify
        /// </summary>
        public IEnumerable<IRecordMatchResult> Classify(IdentifiedData input, IEnumerable<IdentifiedData> blocks, String configurationName, IRecordMatchingDiagnosticSession collector = null)
        {
            var genMethod = this.GetType().GetGenericMethod(nameof(Classify), new Type[] { input.GetType() }, new Type[] { input.GetType(), typeof(IEnumerable<>).MakeGenericType(input.GetType()), typeof(String), typeof(IRecordMatchingDiagnosticSession) });
            var ofTypeMethod = typeof(Enumerable).GetGenericMethod(nameof(Enumerable.OfType), new Type[] { input.GetType() }, new Type[] { typeof(IEnumerable) });
            var results = genMethod.Invoke(this, new object[] { input, ofTypeMethod.Invoke(null, new object[] { blocks }), configurationName, collector }) as IEnumerable;
            return results.OfType<IRecordMatchResult>();
        }

        /// <summary>
        /// Create a diagnostics session
        /// </summary>
        public IRecordMatchingDiagnosticSession CreateDiagnosticSession()
        {
            return new MatchDiagnosticSession();
        }
    }
}