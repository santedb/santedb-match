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
        [XmlArray("args")]
        [XmlArrayItem("int", typeof(int))]
        [XmlArrayItem("double", typeof(double))]
        [XmlArrayItem("string", typeof(string))]
        [XmlArrayItem("boolean", typeof(bool))]
        public List<Object> Parameters { get; set; }
    }
}