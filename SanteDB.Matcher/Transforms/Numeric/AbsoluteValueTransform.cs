/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
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
 * Date: 2020-1-22
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Numeric
{
    /// <summary>
    /// Absolute value transform
    /// </summary>
    public class AbsoluteValueTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name => "abs";

        /// <summary>
        /// Apply the transform to the value
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            if(input is int)
                return Math.Abs((int)input);
            else if (input is double)
                return Math.Abs((double)input);
            throw new InvalidOperationException($"Cannot absolute value of type {input.GetType().Name}");
        }
    }
}
