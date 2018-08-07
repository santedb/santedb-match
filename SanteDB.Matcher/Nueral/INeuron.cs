using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral
{
    /// <summary>
    /// Represents a neuron in the neural network
    /// </summary>
    public interface INeuron
    {
        /// <summary>
        /// Gets the unique id of the neuron
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the previous derivate calculation from the last test run
        /// </summary>
        double PreviousPartialDerivate { get; set; }

        /// <summary>
        /// The input connections to this neuron
        /// </summary>
        List<ISynapse> Inputs { get; set; }

        /// <summary>
        /// The output connections of this neuron
        /// </summary>
        List<ISynapse> Outputs { get; set; }

        /// <summary>
        /// Adds a input neuron to this neuron
        /// </summary>
        /// <param name="inputNeuron">The neuron from which this neuron receives input</param>
        void AddInputNeuron(INeuron inputNeuron);

        /// <summary>
        /// Adds an output neuron to this neuron
        /// </summary>
        /// <param name="outputNeuron">The neuron which receives output from this neuron</param>
        void AddOutputNeuron(INeuron outputNeuron);

        /// <summary>
        /// Calculates the output of the neuron
        /// </summary>
        double CalculateOut();

        /// <summary>
        /// Adds an input synapse
        /// </summary>
        /// <param name="inputValue">The value of the input synapse</param>
        /// <remarks>When this neuron is in the input layer, it receives raw inputs rather than calculated inputs from
        /// other neurons</remarks>
        void AddInputSynapse(double inputValue);

        /// <summary>
        /// Pushes an input value to the input synapse of this neuron
        /// </summary>
        /// <param name="inputValue">The value to push into the input</param>
        void PushInputValue(double inputValue);

    }
}
