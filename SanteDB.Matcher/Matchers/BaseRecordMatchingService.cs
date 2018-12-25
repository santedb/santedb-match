/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
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
 * User: justin
 * Date: 2018-12-25
 */
using System.Collections.Generic;
using SanteDB.Core.Services;
using System.Reflection;
using System.Linq;
using SanteDB.Core.Model.Query;
using System;
using SanteDB.Core;
using SanteDB.Matcher.Model;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Interfaces;
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
    public abstract class BaseRecordMatchingService : IRecordMatchingService
    {
        /// <summary>
        /// Service name
        /// </summary>
        public abstract string ServiceName { get; }

        /// <summary>
        /// Static CTOR
        /// </summary>
        static BaseRecordMatchingService()
        {
            foreach (var t in typeof(BaseRecordMatchingService).GetTypeInfo().Assembly.ExportedTypes.Where(t => typeof(IQueryFilterExtension).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()) && !t.GetTypeInfo().IsAbstract))
                QueryFilterExtensions.AddExtendedFilter(Activator.CreateInstance(t) as IQueryFilterExtension);
        }

        /// <summary>
        /// Perform the record matching services
        /// </summary>
        /// <typeparam name="T">The type of object to be classified</typeparam>
        /// <param name="input">The input to be blocked</param>
        /// <param name="configurationName">The name of the configuration to use</param>
        /// <returns>The blocked records</returns>
        public virtual IEnumerable<T> Block<T>(T input, string configurationName) where T : IdentifiedData
        {

            if (EqualityComparer<T>.Default.Equals(default(T), input)) throw new ArgumentNullException(nameof(input), "Input classifier is required");

            // Get configuration if specified
            var config = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>().GetConfiguration(configurationName);
            config = (config as MatchConfigurationCollection)?.Configurations.FirstOrDefault(o => o.Target.Any(t => typeof(T).GetTypeInfo().IsAssignableFrom(t.ResourceType.GetTypeInfo()))) ?? config;
            if (config == null || !(config is MatchConfiguration))
                throw new InvalidOperationException($"Configuration {config?.GetType().Name ?? "null"} is not compatible with this provider");

            var strongConfig = config as MatchConfiguration;
            if (!strongConfig.Target.Any(t => t.ResourceType.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo())))
                throw new InvalidOperationException($"Configuration {strongConfig.Name} doesn't appear to contain any reference to {typeof(T).FullName}");

            // If the blocking algorithm for this type is AND then we can just use a single IMSI query
            if (strongConfig.Blocking.Count > 0)
            {
                IEnumerable<T> retVal = null;
                foreach (var b in strongConfig.Blocking)
                    if (retVal == null)
                        retVal = this.DoBlock<T>(input, b);
                    else if(b.Operator == BinaryOperatorType.AndAlso)
                        retVal = retVal.Intersect(this.DoBlock<T>(input, b), new IdentifiedComparator<T>());
                    else if (b.Operator == BinaryOperatorType.OrElse)
                        retVal = retVal.Union(this.DoBlock<T>(input, b)).Distinct(new IdentifiedComparator<T>());
                return retVal;
            }
            else
                throw new InvalidOperationException($"Configuration {config.Name} contains no blocking instructions, cannot Block");
        }

        /// <summary>
        /// Perform the block operation
        /// </summary>
        private IEnumerable<T> DoBlock<T>(T input, MatchBlock block) where T : IdentifiedData
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

            // Make LINQ query 
            var linq = QueryExpressionParser.BuildLinqExpression<T>(qfilter, new Dictionary<string, Delegate>()
                {
                    { "input", ((Func<T>)(() => input)) }
                }, true);

            // Total results
            int tr = 0;
            var retVal = persistenceService.Find(linq, 0, block.MaxReuslts, out tr);

            if (tr > block.MaxReuslts)
                throw new InvalidOperationException($"Returned results {tr} exceeds configured maximum of {block.MaxReuslts}");
            return retVal;
        }

        /// <summary>
        /// Classiries the specified blocks into matching results
        /// </summary>
        public abstract IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName) where T : IdentifiedData;

        /// <summary>
        /// Performs a block and match operation in one call
        /// </summary>
        public abstract IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName) where T : IdentifiedData;

        public IRecordMatchResult<T> Score<T>(T input, Expression<Func<T, bool>> query, string configurationName) where T : IdentifiedData
        {
            throw new NotImplementedException();
        }
    }
}