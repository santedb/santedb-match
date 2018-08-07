using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Match transform configuration
    /// </summary>
    [XmlType(nameof(MatchTransform), Namespace = "http://santedb.org/matcher")]
    public class MatchTransform
    {

        /// <summary>
        /// The registered name of the transformer
        /// </summary>
        [XmlAttribute("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the match transformation parameter
        /// </summary>
        [XmlElement("parameter")]
        public List<MatchTransformParameter> Parameters { get; set; }
    }
}