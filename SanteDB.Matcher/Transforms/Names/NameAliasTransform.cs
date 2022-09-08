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
using SanteDB.Matcher.Filters;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace SanteDB.Matcher.Transforms.Names
{
    /// <summary>
    /// Applies a name alias transform
    /// </summary>
    [Description("Transform the name string to a collection of known aliases (i.e. Will = Bill, William, etc.)"), DisplayName("Name Alias Lookup")]
    public class NameAliasTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Getst the transformation
        /// </summary>
        public string Name => "name_alias";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            if (a is string)
                return ((String)a).Alias((String)b);
            else if (a is IEnumerable aEnum && b is IEnumerable bEnum)
                return aEnum.OfType<String>().SelectMany(ar => bEnum.OfType<String>().Select(br => ar.Alias(br)));
            else
                throw new ArgumentException("Cannot execute this transform on this data type");
        }
    }
}