﻿/*
 *
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
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Represents a file path configuration
    /// </summary>
    [XmlType(nameof(FilePathConfiguration), Namespace = "http://santedb.org/configuration")]
    [JsonObject(nameof(FilePathConfiguration))]
    public class FilePathConfiguration
    {

        /// <summary>
        /// True if the path should be treated as readonly
        /// </summary>
        [XmlAttribute("readonly")]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [XmlText, JsonProperty("value")]
        public string Path { get; set; }
    }
}