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
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Represents a match transformer configuration
    /// </summary>
    [XmlType(nameof(MatchTransformConfiguration), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchTransformConfiguration))]
    public class MatchTransformConfiguration
    {

        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the type
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public string TypeXml { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [XmlIgnore]
        public Type Type
        {
            get => Type.GetType(this.TypeXml);
            set => this.TypeXml = value.AssemblyQualifiedName;
        }

    }
}