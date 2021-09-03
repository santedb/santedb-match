﻿/*
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
using SanteDB.Core.Model;
using SanteDB.Core.Model.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Test
{
    public static class ExtensionMethodHelpers
    {

        /// <summary>
        /// Load all concepts for T
        /// </summary>
        public static T LoadConcepts<T>(this T me) where T  : IdentifiedData
        {

            foreach(var p in me.GetType().GetRuntimeProperties())
            {
                var val = p.GetValue(me);
                if (val is IEnumerable)
                    foreach (var v in val as IEnumerable)
                        (v as IdentifiedData)?.LoadConcepts();
                else if (val == null && p.PropertyType == typeof(Concept))
                    me.LoadProperty<Concept>(p.Name);
            }
            return me;
        }
    }
}
