using SanteDB.Matcher.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents a transform which extracts a part of the string
    /// </summary>
    public class SubstringTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "substr";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            if (parms.Length == 1)
                return ((String)input).Substr((int)parms[0]);
            else if (parms.Length == 2)
                return ((String)input).Substr((int)parms[0], (int)parms[1]);
            else
                throw new ArgumentOutOfRangeException("substr transform only supports one or two parameters");
        }
    }
}
