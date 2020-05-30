using SanteDB.Matcher.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.Matcher.Transforms.Names
{
    /// <summary>
    /// Applies a name alias transform
    /// </summary>
    public class NameAliasTransform : IBinaryDataTransformer
    {

        /// <summary>
        /// Getst the transformation
        /// </summary>
        public string Name => "name_alias";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            if (a is string)
                return ((String)a).Alias((String)b);
            else if (a is IEnumerable aEnum && b is IEnumerable bEnum)
                return aEnum.OfType<String>().SelectMany(ar => bEnum.OfType<String>().Select(br => ar.Alias(br)));
            else
                throw new ArgumentException("Cannot execute this transform on this data type");

        }

    }
}
