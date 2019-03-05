/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 *
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
 * User: JustinFyfe
 * Date: 2019-1-22
 */
using SanteDB.Matcher.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents a transform which extracts a part of the string
    /// </summary>
    public class SubstringTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "substr";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            if (parms.Length == 1)
                return ((String)input).Substr((int)parms[0]);
            else if (parms.Length == 2)
                return ((String)input).Substr((int)parms[0], (int)parms[1]);
            else
                throw new ArgumentOutOfRangeException("substr transform only supports one or two parameters");
        }
    }
}
