/*
 *
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2019-11-27
 */
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
