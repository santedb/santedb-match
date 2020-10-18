using SanteDB.Matcher.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Jaro-Winkler text transformation 
    /// </summary>
    public class JaroWinklerTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public String Name => "jaro-winkler";

        /// <summary>
        /// Applies the transform the to specified object
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            if (a is String)
                return ((String)a).JaroWinkler((String)b);
            else if (a is IEnumerable aEnum && b is IEnumerable bEnum)
                return aEnum.OfType<String>().SelectMany(sa => bEnum.OfType<String>().Select(sb => sa.JaroWinkler(sb)));
            else
                throw new InvalidOperationException("Cannot process this transformation on this type of input");

        }
    }
}
