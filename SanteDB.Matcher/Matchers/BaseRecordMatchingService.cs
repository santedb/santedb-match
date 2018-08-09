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

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents base record matching service for SanteDB Matcher
    /// </summary>
    public abstract class BaseRecordMatchingService : IRecordMatchingService
    {

        /// <summary>
        /// Identified comparator
        /// </summary>
        private class IdentifiedComparator<T> : IEqualityComparer<T>
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
            var config = ApplicationServiceContext.Current.GetSerivce<IRecordMatchingConfigurationService>().GetConfiguration(configurationName);
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
                        retVal = this.DoBlock<T>(input, strongConfig.Blocking.First());
                    else if(b.Operator == BinaryOperatorType.AndAlso)
                        retVal = retVal.Intersect(this.DoBlock<T>(input, strongConfig.Blocking.First()), new IdentifiedComparator<T>());
                    else if (b.Operator == BinaryOperatorType.OrElse)
                        retVal = retVal.Union(this.DoBlock<T>(input, strongConfig.Blocking.First())).Distinct(new IdentifiedComparator<T>());
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
            var persistenceService = ApplicationServiceContext.Current.GetSerivce<IRepositoryService<T>>();
            if (persistenceService == null)
                throw new InvalidOperationException($"Cannot find persistence service for {typeof(T).FullName}");

            // Perpare filter
            var filter = block.Filter;
            NameValueCollection qfilter = new NameValueCollection();
            foreach (var b in filter)
            {
                var nvc = NameValueCollection.ParseQueryString(b.Filter);
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
    }
}