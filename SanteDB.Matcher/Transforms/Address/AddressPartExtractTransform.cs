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
 * Date: 2023-5-19
 */
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.Entities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SanteDB.Matcher.Transforms.Names
{
    /// <summary>
    /// Extracts a name part from the specified input
    /// </summary>
    [Description("Extract a part from the address"), DisplayName("Address Part Extract")]
    [TransformArgument(typeof(String), "part", "The part to extract (StreetAddress, City, State, PostalCode, etc.)", true)]
    public class AddressPartExtractTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Get the name
        /// </summary>
        public string Name => "addresspart_extract";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            var en = input as EntityAddress;
            if (en == null)
            {
                throw new ArgumentOutOfRangeException(nameof(input), "This transform requires an EntityAddress to be in scope");
            }

            if (parms.Length != 1)
            {
                throw new ArgumentNullException("partname", "Exactly one name part type must be specified");
            }

            try
            {
                var partUuid = (Guid)(typeof(AddressComponentKeys).GetRuntimeField(parms[0].ToString())?.GetValue(null));
                return en.LoadProperty(o => o.Component).Where(o => o.ComponentTypeKey == partUuid || o.ComponentType?.Mnemonic == parms[0].ToString())?.Select(o => o.Value);
            }
            catch
            {
                throw new InvalidOperationException($"Cannot extract address part {parms[0]}");
            }
        }
    }
}