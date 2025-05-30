﻿/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
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
    [Description("Calculate a Jaro-Winkler value (a difference between A and B) from 0..1"), DisplayName("Jaro-Winkler")]
    public class JaroWinklerTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public String Name => "jaro_winkler";

        /// <summary>
        /// Applies the transform the to specified object
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            if (a is String)
            {
                return ((String)a).JaroWinkler((String)b);
            }
            else if (a is IEnumerable aEnum && b is IEnumerable bEnum)
            {
                return aEnum.OfType<String>().SelectMany(sa => bEnum.OfType<String>().Select(sb => sa.JaroWinkler(sb)));
            }
            else if (a is IdentifiedData aId && b is IdentifiedData bId)
            {
                return aId.ToDisplay().SimilarityTo(bId.ToDisplay());
            }
            else
            {
                throw new InvalidOperationException("Cannot process this transformation on this type of input");
            }
        }
    }
}