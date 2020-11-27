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
using SanteDB.Matcher.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents the soundex transform
    /// </summary>
    public class SoundexTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "soundex";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            if (input is String inputString)
                return inputString.Soundex();
            else if (input is IEnumerable inputEnum)
                return inputEnum.OfType<String>().Select(o => o.Soundex());
            else
                throw new InvalidOperationException("Cannot process this transformation on this type of input");
        }
    }
}
