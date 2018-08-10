using SanteDB.Matcher.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents the metaphone transform
    /// </summary>
    public class MetaphoneTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "metaphone";

        /// <summary>
        /// Returns the metaphone code
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            return ((String)input).Metaphone();
        }
    }
}
