﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SanteDB.Core.Configuration;

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
