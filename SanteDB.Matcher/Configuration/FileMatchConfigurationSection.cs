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
using Newtonsoft.Json;
using SanteDB.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Configures the file matching configuration
    /// </summary>
    [XmlType(nameof(FileMatchConfigurationSection), Namespace = "http://santedb.org/configuration")]
    [JsonObject(nameof(FileMatchConfigurationSection))]
    public class FileMatchConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// File path
        /// </summary>
        [XmlArray("basePath"), XmlArrayItem("add"), JsonProperty("basePath")]
        public List<FilePathConfiguration> FilePath { get; set; }

    }
}