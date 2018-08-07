using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral
{
    /// <summary>
    /// Represents an activation function
    /// </summary>
    public interface IActivationFunction
    {

        /// <summary>
        /// Calculates the output activation of a nueron in the network
        /// </summary>
        /// <param name="input">The input value</param>
        /// <returns>The output of the activation function</returns>
        double CalculateOut(double input);
    }
}
