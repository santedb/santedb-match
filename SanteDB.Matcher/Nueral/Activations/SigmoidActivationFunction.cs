using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral.Activations
{
    /// <summary>
    /// Sigmoid activation functions
    /// </summary>
    public class SigmoidActivationFunction : IActivationFunction
    {
        private double m_coefficient;

        /// <summary>
        /// Constructs a new sigmoid activation function
        /// </summary>
        /// <param name="coefficient">The coefficient to the function</param>
        public SigmoidActivationFunction(double coefficient)
        {
            this.m_coefficient = coefficient;
        }

        /// <summary>
        /// Calculate the output
        /// </summary>
        public double CalculateOut(double input)
        {
            return (1 / (1 + Math.Exp(-input * this.m_coefficient)));
        }


    }
}
