using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents a probabalistic record matching service
    /// </summary>
    public class ProbabalisticRecordMatchingService : IRecordMatchingService
    {

        /// <summary>
        /// Perform blocking using the current database layer of resources matching the specified IMSI expressions
        /// </summary>
        /// <typeparam name="T">The type of data we're blocking</typeparam>
        /// <param name="input">The input object to block against</param>
        /// <param name="configurationName">The configuration name</param>
        /// <returns>Blocked input</returns>
        public IEnumerable<T> Block<T>(T input, string configurationName)
        {
            throw new NotImplementedException();
        }

        public bool Classify<T>(T input, IEnumerable<T> blocks, string configurationName, out IEnumerable<T> matched, out IEnumerable<T> probable, out IEnumerable<T> discard)
        {
            throw new NotImplementedException();
        }

        public bool Match<T>(T input, string configurationName, out IEnumerable<T> matched, out IEnumerable<T> probable, out IEnumerable<T> discard)
        {
            throw new NotImplementedException();
        }
    }
}
