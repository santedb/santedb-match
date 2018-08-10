using SanteDB.Matcher.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents a transform which reports the similarity
    /// </summary>
    public class SimilarityTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "similarity";

        /// <summary>
        /// Applies the filter
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            return ((String)a).SimilarityTo((String)b);
        }
    }
}
