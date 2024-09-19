/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Metadata tags
    /// </summary>
    [XmlType(nameof(MatchConfigurationMetadataTag), Namespace = "http://santedb.org/matcher")]
    public class MatchConfigurationMetadataTag
    {

        /// <summary>
        /// Gets or sets the tags
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// The value of the tags
        /// </summary>
        [XmlText, JsonProperty("value")]
        public string Value { get; set; }
    }
}