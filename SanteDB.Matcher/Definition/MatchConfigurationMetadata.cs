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
 * Date: 2023-3-10
 */
using Newtonsoft.Json;
using SanteDB.Core.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Represents match configuration metadata in XML
    /// </summary>
    [XmlType(nameof(MatchConfigurationMetadata), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchConfigurationMetadata))]
    public class MatchConfigurationMetadata : IRecordMatchingConfigurationMetadata
    {
        /// <summary>
        /// Create new serialization instance of metadata
        /// </summary>
        public MatchConfigurationMetadata()
        {
            this.Tags = new List<MatchConfigurationMetadataTag>();
        }

        /// <summary>
        /// Copy data from <paramref name="value"/> into this instance
        /// </summary>
        public MatchConfigurationMetadata(IRecordMatchingConfigurationMetadata value)
        {
            this.Version = 1;
            this.CreatedBy = value.CreatedBy;
            this.CreationTime = value.CreationTime.DateTime;
            this.Status = value.Status;

            this.Tags = new List<MatchConfigurationMetadataTag>(value.Tags.Select(o => new MatchConfigurationMetadataTag()
            {
                Key = o.Key,
                Value = o.Value
            }));
        }

        /// <summary>
        /// Created by whom?
        /// </summary>
        [XmlElement("createdBy"), JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation time
        /// </summary>
        [XmlElement("creationTime"), JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [XmlElement("status"), JsonProperty("status")]
        public MatchConfigurationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        [XmlElement("version"), JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the tags for the data
        /// </summary>
        [XmlIgnore, JsonIgnore]
        IDictionary<string, string> IRecordMatchingConfigurationMetadata.Tags => this.Tags.ToDictionary(o => o.Key, o => o.Value);

        /// <summary>
        /// Add or remove tag information for serialization
        /// </summary>
        [XmlArray("tags"), XmlArrayItem("add"), JsonProperty("tags")]
        public List<MatchConfigurationMetadataTag> Tags { get; set; }

        /// <summary>
        /// Gets or sets the creation time
        /// </summary>
        [XmlElement("updatedTime"), JsonProperty("updatedTime")]
        public DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// Gets or sets the updated person
        /// </summary>
        [XmlElement("updatedBy"), JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// If this is readonly
        /// </summary>
        [XmlElement("isReadonly"), JsonProperty("isReadonly")]
        public bool IsReadonly { get; set; }

        /// <summary>
        /// Gets or sets the description of the match configuration
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        DateTimeOffset IRecordMatchingConfigurationMetadata.CreationTime => this.CreationTime;

        /// <summary>
        /// Gets the updated time
        /// </summary>
        DateTimeOffset? IRecordMatchingConfigurationMetadata.UpdatedTime => this.UpdatedTime;
    }
}