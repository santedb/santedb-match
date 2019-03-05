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
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Names
{
    /// <summary>
    /// Extracts a name part from the specified input
    /// </summary>
    public class NamePartExtractTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Get the name
        /// </summary>
        public string Name => "namepart_extract";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            var en = input as EntityName;
            if (en == null) throw new ArgumentOutOfRangeException(nameof(input), "This transform requires an EntityName to be in scope");
            if (parms.Length != 1) throw new ArgumentNullException("partname", "Exactly one name part type must be specified");
            try
            {
                var partUuid = (Guid)(typeof(NameComponentKeys).GetRuntimeField(parms[0].ToString())?.GetValue(null));
                return en.Component.FirstOrDefault(o => o.ComponentTypeKey == partUuid || o.ComponentType?.Mnemonic == parms[0].ToString());
            }
            catch
            {
                throw new InvalidOperationException($"Cannot extract name part {parms[0]}");
            }
        }
    }
}
