using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral.Activations
{
    /// <summary>
    /// Represetns a rectifier activation function based on maximum value
    /// </summary>
    public class RectifiedActivationFunction : IActivationFunction
    {
        /// <summary>
        /// Calculate output
        /// </summary>
        public double CalculateOut(double input)
        {
            return Math.Max(0, input);
        }
    }
}
