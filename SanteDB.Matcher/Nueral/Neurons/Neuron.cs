using SanteDB.Matcher.Nueral.Synapses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral.Neurons
{
    /// <summary>
    /// Implementation of a neuron
    /// </summary>
    public class Neuron : INeuron
    {
        // ID of this Neuron
        private readonly Guid m_id = Guid.NewGuid();
        private readonly IActivationFunction m_activationFunction;
        private readonly IInputFunction m_inputFunction;

        /// <summary>
        /// Get the ID of the neuron
        /// </summary>
        public Guid Id => this.m_id;

        /// <summary>
        /// Calculated partial derivat in the previous iteration of training
        /// </summary>
        public double PreviousPartialDerivate { get; set; }

        /// <summary>
        /// Gets the inputs of the neuron
        /// </summary>
        public List<ISynapse> Inputs { get; set; }

        /// <summary>
        /// Gets the outputs of the neruon
        /// </summary>
        public List<ISynapse> Outputs { get; set; }

        /// <summary>
        /// Creates a new neuron
        /// </summary>
        public Neuron(IActivationFunction activationFunction, IInputFunction inputFunction)
        {
            this.Inputs = new List<ISynapse>();
            this.Outputs = new List<ISynapse>();
            this.m_activationFunction = activationFunction;
            this.m_inputFunction = inputFunction;
        }

        /// <summary>
        /// Connects this neuron to another
        /// </summary>
        /// <param name="inputNeuron">The neuron to treat as input to this neuron</param>
        public void AddInputNeuron(INeuron inputNeuron)
        {
            var synapse = new Synapse(inputNeuron, this);
            this.Inputs.Add(synapse);
            inputNeuron.Outputs.Add(synapse);
        }

        /// <summary>
        /// Adds an input value to the network
        /// </summary>
        /// <param name="inputValue">The input value</param>
        public void AddInputSynapse(double inputValue)
        {
            var synapse = new InputSynapse(this, inputValue);
            this.Inputs.Add(synapse);
        }

        /// <summary>
        /// Connects this neuron to another 
        /// </summary>
        /// <param name="outputNeuron">The neuron that will be the output of this connection</param>
        public void AddOutputNeuron(INeuron outputNeuron)
        {
            var synapse = new Synapse(this, outputNeuron);
            this.Inputs.Add(synapse);
            outputNeuron.Inputs.Add(synapse);
        }

        /// <summary>
        /// Calculates the output value of this neuron
        /// </summary>
        public double CalculateOut()
        {
            return this.m_activationFunction.CalculateOut(this.m_inputFunction.CalculateIn(this.Inputs));
        }

        /// <summary>
        /// Values from the input layer of the neural network. These are the outputs of the matching vector
        /// </summary>
        /// <param name="inputValue">The input value</param>
        public void PushInputValue(double inputValue)
        {
            this.Inputs.OfType<InputSynapse>().First().Output = inputValue;
        }
    }
}
