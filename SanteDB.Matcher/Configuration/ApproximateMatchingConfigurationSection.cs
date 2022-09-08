/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2022-5-30
 */
using Newtonsoft.Json;
using SanteDB.Core.Configuration;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Approximate matching configuration section
    /// </summary>
    [XmlType(nameof(ApproximateMatchingConfigurationSection), Namespace = "http://santedb.org/configuration")]
    [JsonObject(nameof(ApproximateMatchingConfigurationSection))]
    public class ApproximateMatchingConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// Gets or sets the search options for the approx() operator
        /// </summary>
        [XmlArray("approxOptions"),
            XmlArrayItem("phonetic", Type = typeof(ApproxPhoneticOption)),
            XmlArrayItem("difference", Type = typeof(ApproxDifferenceOption)),
            XmlArrayItem("pattern", Type = typeof(ApproxPatternOption)),
            JsonProperty("approxOptions")]
        public List<ApproxSearchOption> ApproxSearchOptions { get; set; }

    }


    /// <summary>
    /// Approximate search options
    /// </summary>
    [XmlType(nameof(ApproxSearchOption)), JsonObject(nameof(ApproxSearchOption))]
    public abstract class ApproxSearchOption
    {
        /// <summary>
        /// True if the option is enabled
        /// </summary>
        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }

    }

    /// <summary>
    /// Phonetic options
    /// </summary>
    [XmlType(nameof(ApproxPhoneticOption)), JsonObject(nameof(ApproxPhoneticOption))]
    public class ApproxPhoneticOption : ApproxSearchOption
    {

        /// <summary>
        /// Phonetic algorithm types
        /// </summary>
        [XmlType(nameof(PhoneticAlgorithmType))]
        public enum PhoneticAlgorithmType
        {
            /// <summary>
            /// Let the server or persistence engine decide
            /// </summary>
            [XmlEnum("auto")]
            Auto,
            /// <summary>
            /// Use soundex matching
            /// </summary>
            [XmlEnum("soundex")]
            Soundex,
            /// <summary>
            /// Use metaphone matching
            /// </summary>
            [XmlEnum("metaphone")]
            Metaphone,
            /// <summary>
            /// Use dmetaphone matching
            /// </summary>
            [XmlEnum("dmetaphone")]
            DoubleMetaphone
        }

        /// <summary>
        /// The phonetic algorithm type
        /// </summary>
        [XmlAttribute("algorithm"), JsonProperty("algorithm")]
        public PhoneticAlgorithmType Algorithm { get; set; }

        /// <summary>
        /// Similarity
        /// </summary>
        [XmlAttribute("similarity"), JsonProperty("similarity")]
        public float MinSimilarity { get; set; }

        /// <summary>
        /// Was it specified
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool MinSimilaritySpecified { get; set; }
    }

    /// <summary>
    /// Difference option
    /// </summary>
    [XmlType(nameof(ApproxDifferenceOption)), JsonObject(nameof(ApproxDifferenceOption))]
    public class ApproxDifferenceOption : ApproxSearchOption
    {
        /// <summary>
        /// Similarity
        /// </summary>
        [XmlAttribute("max"), JsonProperty("max")]
        public float MaxDifference { get; set; }

    }

    /// <summary>
    /// Pattern option
    /// </summary>
    [XmlType(nameof(ApproxPatternOption)), JsonObject(nameof(ApproxPatternOption))]
    public class ApproxPatternOption : ApproxSearchOption
    {

        /// <summary>
        /// Identifies the system should ignore case
        /// </summary>
        [XmlAttribute("ignoreCase"), JsonProperty("ignoreCase")]
        public bool IgnoreCase { get; set; }
    }

}
