/*
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
 * Date: 2019-11-27
 */
using Newtonsoft.Json;
using SanteDB.Core.Model.Serialization;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Gets the matching configuration
    /// </summary>
    [XmlType(nameof(MatchConfiguration), Namespace = "http://santedb.org/matcher")]
    [XmlRoot(nameof(MatchConfiguration), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchConfiguration))]
    public class MatchConfiguration : IRecordMatchingConfiguration
    {

        /// <summary>
        /// Gets or sets the name of the matching configuration
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the score at which a record is not a match
        /// </summary>
        [XmlAttribute("nonmatchThreshold"), JsonProperty("nonmatchThreshold")]
        public double NonMatchThreshold { get; set; }

        /// <summary>
        /// Gets or sets the score at which a match is considered so
        /// </summary>
        [XmlAttribute("matchThreshold"), JsonProperty("matchThreshold")]
        public double MatchThreshold { get; set; }

        /// <summary>
        /// Gets or sets the targets this configuration applies to
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        public List<MatchTarget> Target { get; set; }

        /// <summary>
        /// Gets or sets the blocking configuration
        /// </summary>
        [XmlElement("blocking"), JsonProperty("blocking")]
        public List<MatchBlock> Blocking { get; set; }

        /// <summary>
        /// Gets or sets the classifications
        /// </summary>
        [XmlArray("scoring")]
        [XmlArrayItem("attribute")]
        [JsonProperty("scoring")]
        public List<MatchAttribute> Classification { get; set; }

        /// <summary>
        /// Load match configuration from the specified stream
        /// </summary>
        /// <param name="s">The stream to load from</param>
        /// <returns>The loaded match configuration</returns>
        public static MatchConfiguration Load(Stream s)
        {
            var xs = XmlModelSerializerFactory.Current.CreateSerializer(typeof(MatchConfiguration));
            return xs.Deserialize(s) as MatchConfiguration;
        }

        /// <summary>
        /// Save this match configuration collection to the specified stream
        /// </summary>
        public void Save(Stream s)
        {
            var xs = XmlModelSerializerFactory.Current.CreateSerializer(typeof(MatchConfiguration));
            xs.Serialize(s, this);
        }
    }
}
