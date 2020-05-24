﻿/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
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
 * Date: 2020-1-1
 */
using SanteDB.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{

    /// <summary>
    /// A configuration section for the file configuraiton provider
    /// </summary>
    [XmlType(nameof(AssemblyMatchConfigurationSection), Namespace = "http://santedb.org/configuration")]
    public class AssemblyMatchConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// Gets the base path from which the file based configuration should be loaded
        /// </summary>
        [XmlArray("assemblies"), XmlArrayItem("add")]
        public List<String> Assemblies { get; set; }


    }
}
