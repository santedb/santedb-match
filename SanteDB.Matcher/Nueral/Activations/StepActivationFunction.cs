using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral.Activations
{
    /// <summary>
    /// Calculates the activation based on threshold
    /// </summary>
    public class StepActivationFunction : IActivationFunction
    {
        private double m_threshold;

        /// <summary>
        /// The threshold for activation
        /// </summary>
        /// <param name="threshold">The threshold</param>
        public StepActivationFunction(double threshold)
        {
            this.m_threshold = threshold;
        }

        /// <summary>
        /// Calculate the output (1.0 is active 0.0 if inactive)
        /// </summary>
        public double CalculateOut(double input)
        {
            return Convert.ToDouble(input > this.m_threshold);
        }
    }
}
