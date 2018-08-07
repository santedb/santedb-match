using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Nueral
{
    /// <summary>
    /// Represents an input function to the network
    /// </summary>
    public interface IInputFunction
    {
        /// <summary>
        /// Calculates the inputs 
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        double CalculateIn(List<ISynapse> inputs);
    }
}
