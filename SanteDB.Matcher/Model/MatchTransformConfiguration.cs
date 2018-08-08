using System;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a match transformer configuration
    /// </summary>
    [XmlType(nameof(MatchTransformConfiguration), Namespace = "http://santedb.org/matcher")]
    public class MatchTransformConfiguration
    {

        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the type
        /// </summary>
        [XmlAttribute("type")]
        public string TypeXml { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [XmlIgnore]
        public Type Type
        {
            get => Type.GetType(this.TypeXml);
            set => this.TypeXml = value.AssemblyQualifiedName;
        }

    }
}