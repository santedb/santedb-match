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
using SanteDB.Core.Model.Query;
using SanteDB.Matcher.Filters;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents a transform which extracts a part of the string
    /// </summary>
    [Description("Extract a portion of the string"), DisplayName("Substring")]
    [TransformArgument(typeof(int), "start", "The offset in the string to start extraction", true)]
    [TransformArgument(typeof(int), "length", "The length of characters to extract from the string (if not provided it is until end of string)", false)]
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
            {
                if (input is String inputString)
                    return (inputString). Substr((int)parms[0], null);
                else if (input is IEnumerable inputEnum)
                    return inputEnum.OfType<string>().Select(o => o.Substr((int)parms[0], null));
                else
                    throw new InvalidOperationException("Cannot process this transformation on this type of input");
            }
            else if (parms.Length == 2)
            {
                if (input is String inputString)
                    return (inputString).Substr((int)parms[0], (int)parms[1]);
                else if (input is IEnumerable inputEnum)
                    return inputEnum.OfType<string>().Select(o => o.Substr((int)parms[0], (int)parms[1]));
                else
                    throw new InvalidOperationException("Cannot process this transformation on this type of input");
            }
            else
                throw new ArgumentOutOfRangeException("substr transform only supports one or two parameters");
        }
    }
}