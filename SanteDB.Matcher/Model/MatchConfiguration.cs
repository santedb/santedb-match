using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Gets the matching configuration
    /// </summary>
    [XmlType(nameof(MatchConfiguration), Namespace = "http://santedb.org/matcher")]
    [XmlRoot(nameof(MatchConfiguration), Namespace = "http://santedb.org/matcher")]
    public class MatchConfiguration : IRecordMatchingConfiguration
    {

        /// <summary>
        /// Gets or sets the name of the matching configuration
        /// </summary>
        [XmlAttribute("id")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the score at which a record is not a match
        /// </summary>
        [XmlAttribute("nonmatchThreshold")]
        public double NonMatchThreshold { get; set; }

        /// <summary>
        /// Gets or sets the score at which a match is considered so
        /// </summary>
        [XmlAttribute("matchThreshold")]
        public double MatchThreshold { get; set; }

        /// <summary>
        /// Gets or sets the targets this configuration applies to
        /// </summary>
        [XmlElement("target")]
        public List<MatchTarget> Target { get; set; }

        /// <summary>
        /// Gets or sets the blocking configuration
        /// </summary>
        [XmlElement("blocking")]
        public List<MatchBlock> Blocking { get; set; }

        /// <summary>
        /// Gets or sets the classifications
        /// </summary>
        [XmlArray("classification")]
        [XmlArrayItem("vector")]
        public List<MatchVector> Classification { get; set; }

        /// <summary>
        /// Load match configuration from the specified stream
        /// </summary>
        /// <param name="s">The stream to load from</param>
        /// <returns>The loaded match configuration</returns>
        public static MatchConfiguration Load(Stream s)
        {
            var xs = new XmlSerializer(typeof(MatchConfiguration));
            return xs.Deserialize(s) as MatchConfiguration;
        }

        /// <summary>
        /// Save this match configuration collection to the specified stream
        /// </summary>
        public void Save(Stream s)
        {
            var xs = new XmlSerializer(typeof(MatchConfiguration));
            xs.Serialize(s, this);
        }
    }
}
