using SanteDB.Matcher.Nueral.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral
{
    /// <summary>
    /// Represents a layer of neurons
    /// </summary>
    public class NeuralLayer : List<INeuron>
    {

        /// <summary>
        /// Creates a new neural layer
        /// </summary>
        public NeuralLayer()
        {
        }

        /// <summary>
        /// Creates a new neural layer
        /// </summary>
        /// <param name="size">The size of the neural network layer</param>
        /// <param name="activationFunction">The activation function nodes at this layer should have</param>
        /// <param name="inputFunction">The input functions that should be used</param>
        public NeuralLayer(int size, IActivationFunction activationFunction, IInputFunction inputFunction) : base(size)
        {
            this.AddRange(Enumerable.Range(0, size).Select(o => new Neuron(activationFunction, inputFunction)));
        }

        /// <summary>
        /// Connects <paramref name="inputLayer"/> to this layer
        /// </summary>
        /// <param name="inputLayer">The input layer to connect</param>
        public void Connect(NeuralLayer inputLayer)
        {
            var combos = this.SelectMany(n => inputLayer, (n, i) => new { Neuron = n, Input = i });
            foreach (var c in combos)
                c.Neuron.AddInputNeuron(c.Input);
        }
    }
}
