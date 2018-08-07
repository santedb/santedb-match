using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral.Inputs
{
    /// <summary>
    /// Weighted sum function
    /// </summary>
    public class WeightedSum : IInputFunction
    {
        /// <summary>
        /// Calculate the input using weighted sum
        /// </summary>
        /// <param name="inputs">The list of inputs from the previous layer of the network</param>
        /// <returns>The input</returns>
        public double CalculateIn(List<ISynapse> inputs)
        {
            return inputs.Select(o => o.Weight * o.GetOut()).Sum();
        }
    }
}
