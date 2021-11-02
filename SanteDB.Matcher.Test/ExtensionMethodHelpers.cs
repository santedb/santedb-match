/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */

using System.Collections;
using System.Reflection;
using SanteDB.Core.Model;
using SanteDB.Core.Model.DataTypes;

namespace SanteDB.Matcher.Test
{
    public static class ExtensionMethodHelpers
    {
        /// <summary>
        /// Load all concepts for a give source.
        /// </summary>
        public static T LoadConcepts<T>(this T source) where T : IdentifiedData
        {
            foreach (var propertyInfo in source.GetType().GetRuntimeProperties())
            {
                var val = propertyInfo.GetValue(source);

                switch (val)
                {
                    case IEnumerable enumerable:
                    {
                        foreach (var v in enumerable)
                        {
                            (v as IdentifiedData)?.LoadConcepts();
                        }

                        break;
                    }
                    case null when propertyInfo.PropertyType == typeof(Concept):
                        source.LoadProperty<Concept>(propertyInfo.Name);
                        break;
                }
            }

            return source;
        }
    }
}