using SanteDB.Matcher.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents a string difference transformation
    /// </summary>
    public class LevenshteinDifferenceTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public String Name => "levenshtein";

        /// <summary>
        /// Applies the transform the to specified object
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            return ((String)a).Levenshtein((String)b);
        }
    }
}
