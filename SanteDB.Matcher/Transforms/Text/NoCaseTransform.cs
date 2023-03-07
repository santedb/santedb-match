/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2022-5-30
 */
using SanteDB.Core.Model;
using SanteDB.Matcher.Util;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Jaro-Winkler text transformation
    /// </summary>
    [Description("Removes casing from both inputs"), DisplayName("Nocase")]
    public class NocaseTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public String Name => "nocase";

        /// <summary>
        /// Apply the tokenizer
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
           if(input is string s)
            {
                return s.ToLowerInvariant();
            }
            else if (input is IEnumerable inputEnum)
            {
                return inputEnum.OfType<String>().Select(o => o.ToLowerInvariant());
            }
            else
            {
                throw new InvalidOperationException("Cannot process this transformation on this type of input");
            }
        }
    }
}