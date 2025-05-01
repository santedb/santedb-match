/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using Newtonsoft.Json;
using SanteDB.Core.Matching;
using SanteDB.Core.Model.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Represents a collection of match configurations
    /// </summary>
    [XmlType(nameof(MatchConfigurationCollection), Namespace = "http://santedb.org/matcher")]
    [XmlRoot(nameof(MatchConfigurationCollection), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchConfigurationCollection))]
    public class MatchConfigurationCollection : IRecordMatchingConfiguration
    {
        /// <summary>
        /// Create new match configuration collection
        /// </summary>
        public MatchConfigurationCollection()
        {
            this.Configurations = new List<MatchConfiguration>();
            this.Transforms = new List<MatchTransformConfiguration>();
        }

        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets the name of the matching configuration collection
        /// </summary>
        [XmlAttribute("uuid"), JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        /// <summary>
        /// Applies to
        /// </summary>
        public Type[] AppliesTo => this.Configurations.SelectMany(o => o.AppliesTo).ToArray();

        /// <summary>
        /// Adds a transformer to the collection
        /// </summary>
        [XmlArray("transformers")]
        [XmlArrayItem("add")]
        [JsonProperty("transformers")]
        public List<MatchTransformConfiguration> Transforms { get; set; }

        /// <summary>
        /// Gets or sets the configurations for the match collection
        /// </summary>
        [XmlElement("configuration"), JsonProperty("configuration")]
        public List<MatchConfiguration> Configurations { get; set; }

        /// <summary>
        /// Gets or sets the metadata for this object
        /// </summary>
        [XmlIgnore, JsonIgnore]
        IRecordMatchingConfigurationMetadata IRecordMatchingConfiguration.Metadata { get => this.Metadata; set => this.Metadata = new MatchConfigurationMetadata(value); }

        /// <summary>
        /// Gets or sets meta data
        /// </summary>
        [XmlElement("meta"), JsonProperty("meta")]
        public MatchConfigurationMetadata Metadata { get; set; }

        /// <summary>
        /// Load match configuration from the specified stream
        /// </summary>
        /// <param name="s">The stream to load from</param>
        /// <returns>The loaded match configuration</returns>
        public static MatchConfigurationCollection Load(Stream s)
        {
            var xs = XmlModelSerializerFactory.Current.CreateSerializer(typeof(MatchConfigurationCollection));
            return xs.Deserialize(s) as MatchConfigurationCollection;
        }

        /// <summary>
        /// Save this match configuration collection to the specified stream
        /// </summary>
        public void Save(Stream s)
        {
            var xs = XmlModelSerializerFactory.Current.CreateSerializer(typeof(MatchConfigurationCollection));
            xs.Serialize(s, this);
        }
    }
}