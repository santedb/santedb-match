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
using SanteDB.Matcher.Util;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Represents a string difference transformation
    /// </summary>
    [Description("Extract the levenshtein string difference between the input and blocked record"), DisplayName("Levenshtein String Difference")]
    public class LevenshteinDifferenceTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public String Name => "levenshtein";

        /// <summary>
        /// Applies the transform the to specified object
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            if (a is String)
            {
                return ((String)a).Levenshtein((String)b);
            }
            else if (a is IEnumerable aEnum && b is IEnumerable bEnum)
            {
                return aEnum.OfType<String>().SelectMany(sa => bEnum.OfType<String>().Select(sb => sa.Levenshtein(sb)));
            }
            else
            {
                throw new InvalidOperationException("Cannot process this transformation on this type of input");
            }
        }
    }
}