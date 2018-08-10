using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SanteDB.Core.Services;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents a deterministic record matching service
    /// </summary>
    public class DeterministicRecordMatchingService : BaseRecordMatchingService
    {
        /// <summary>
        /// Classify the specified inputs
        /// </summary>
        /// <remarks>This particular record matching service only uses the blocking portion of configuration so all blocked records are considered matches</remarks>
        public override IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName)
        {
            return blocks.Select(o => new MatchResult<T>(o, 1.0, RecordMatchClassification.Match));
        }

        /// <summary>
        /// Performs a block and match operation
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName)
        {
            return this.Classify(input, this.Block(input, configurationName), configurationName);
        }
    }
}
