/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Takes a name and tokenizes it
    /// </summary>
    [Description("Tokenize the input strings based on the provided delimeter"), DisplayName("Tokenize")]
    [TransformArgument(typeof(String), "delimiter", "Characters which should be used to tokenize the string", true)]
    public class TextTokenizeTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Get the name of the transformer
        /// </summary>
        public string Name => "tokenize_extract";

        /// <summary>
        /// Apply the tokenizer
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            if (parms.Length != 1)
            {
                throw new ArgumentException("Require token delimiter parameter");
            }

            // Processed tokens for the name
            List<String> tokens = new List<string>() { (String)input };
            foreach (char delim in (String)parms[0])
            {
                tokens = tokens.SelectMany(o => o.Split(delim)).ToList();
            }
            return tokens;
        }
    }
}