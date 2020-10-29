using SanteDB.Matcher.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Calculates the sorensen dice difference
    /// </summary>
    public class SorensenDiceTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "sorensen_dice";
        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            if (a is String)
                return ((String)a).SorensenDice((String)b);
            else if (a is IEnumerable aEnum && b is IEnumerable bEnum)
                return aEnum.OfType<String>().SelectMany(sa => bEnum.OfType<String>().Select(sb => sa.SorensenDice(sb)));
            else
                throw new InvalidOperationException("Cannot process this transformation on this type of input");

        }
    }
}
