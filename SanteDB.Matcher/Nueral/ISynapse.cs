using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral
{
    /// <summary>
    /// Represents a synapse which is a connection on the network
    /// </summary>
    public interface ISynapse
    {
        /// <summary>
        /// Gets or sets the weight that this synapse has
        /// </summary>
        double Weight { get; set; }

        /// <summary>
        /// Gets or sets the previous weight of this synapse
        /// </summary>
        double PreviousWeight { get; set; }

        /// <summary>
        /// Gets the calculated output of this synapse
        /// </summary>
        /// <returns></returns>
        double GetOut();
        
        /// <summary>
        /// Determines whether the weight is from a neuron
        /// </summary>
        /// <param name="neuronId">The id of the input neuron to test</param>
        bool IsInputNeuron(Guid neuronId);

        /// <summary>
        /// Updates the current weight and stores the previous weight
        /// </summary>
        void UpdateWeight(double learnRate, double delta);
    }
}
