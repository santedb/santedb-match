using System;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Match transformation parameter
    /// </summary>
    [XmlType(nameof(MatchTransformParameter), Namespace = "http://santedb.org/matcher")]
    public class MatchTransformParameter
    {

        /// <summary>
        /// The name of the parameter
        /// </summary>
        [XmlAttribute("name")]
        public String Name { get; set; }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        [XmlElement("valueInt", typeof(int))]
        [XmlElement("valueDouble", typeof(double))]
        [XmlElement("valueString", typeof(string))]
        [XmlElement("valueBoolean", typeof(bool))]
        public Object Value { get; set; }

    }
}