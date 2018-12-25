/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: justin
 * Date: 2018-12-25
 */
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a collection of match configurations
    /// </summary>
    [XmlType(nameof(MatchConfigurationCollection), Namespace = "http://santedb.org/matcher")]
    [XmlRoot(nameof(MatchConfigurationCollection), Namespace = "http://santedb.org/matcher")]
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
        /// Gets the name of the matching configuration collection
        /// </summary>
        [XmlAttribute("id")]
        public string Name { get; set; }

        /// <summary>
        /// Adds a transformer to the collection
        /// </summary>
        [XmlArray("transformers")]
        [XmlArrayItem("add")]
        public List<MatchTransformConfiguration> Transforms { get; set; }

        /// <summary>
        /// Gets or sets the configurations for the match collection
        /// </summary>
        [XmlElement("configuration")]
        public List<MatchConfiguration> Configurations { get; set; }

        /// <summary>
        /// Load match configuration from the specified stream
        /// </summary>
        /// <param name="s">The stream to load from</param>
        /// <returns>The loaded match configuration</returns>
        public static MatchConfigurationCollection Load(Stream s)
        {
            var xs = new XmlSerializer(typeof(MatchConfigurationCollection));
            return xs.Deserialize(s) as MatchConfigurationCollection;
        }

        /// <summary>
        /// Save this match configuration collection to the specified stream
        /// </summary>
        public void Save(Stream s)
        {
            var xs = new XmlSerializer(typeof(MatchConfigurationCollection));
            xs.Serialize(s, this);
        }
    }
}
