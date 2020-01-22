using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Numeric
{
    /// <summary>
    /// Absolute value transform
    /// </summary>
    public class AbsoluteValueTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name => "abs";

        /// <summary>
        /// Apply the transform to the value
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            if(input is int)
                return Math.Abs((int)input);
            else if (input is double)
                return Math.Abs((double)input);
            throw new InvalidOperationException($"Cannot absolute value of type {input.GetType().Name}");
        }
    }
}
