using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms
{
    /// <summary>
    /// Represents a class which can be used to transform input data 
    /// </summary>
    public interface IDataTransformer
    {

        /// <summary>
        /// Get the name of the transform
        /// </summary>
        String Name { get; }

    }

    /// <summary>
    /// Represents a data transformer that works with one input
    /// </summary>
    public interface IUnaryDataTransformer : IDataTransformer
    { 
        /// <summary>
        /// Apply the transformer
        /// </summary>
        /// <param name="input">The input data</param>
        /// <param name="parms">The parameters to the data transformer</param>
        /// <returns>The output data</returns>
        Object Apply(Object input, params Object[] parms);
    }

    /// <summary>
    /// Represents a data transformer that works with two inputs
    /// </summary>
    public interface IBinaryDataTransformer : IDataTransformer
    {
        /// <summary>
        /// Apply the transformer
        /// </summary>
        /// <param name="a">The first input</param>
        /// <param name="b">The second input</param>
        /// <param name="parms">The parameters to the data transformer</param>
        /// <returns>The output data</returns>
        Object Apply(Object a, Object b, params Object[] parms);
    }
}
