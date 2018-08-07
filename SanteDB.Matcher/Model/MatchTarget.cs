using SanteDB.Core.Model.Roles;
using System;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a target of a match (if applicable)
    /// </summary>
    [XmlType(nameof(MatchTarget), Namespace = "http://santedb.org/matcher")]
    public class MatchTarget
    {

        // The type 
        private Type m_type = null;

        /// <summary>
        /// Gets or sets the resource xml
        /// </summary>
        [XmlAttribute("resource")]
        public string ResourceXml { get; set; }

        /// <summary>
        /// The resource type
        /// </summary>
        [XmlIgnore]
        public Type ResourceType {
            get
            {
                if (this.m_type == null)
                    this.m_type = typeof(Patient).GetTypeInfo().Assembly.ExportedTypes.FirstOrDefault(o => o.GetTypeInfo().GetCustomAttribute<XmlRootAttribute>()?.ElementName == this.ResourceXml);
                return this.m_type;
            }
        }

        /// <summary>
        /// The event on which the auto-matching should be triggered
        /// </summary>
        [XmlElement("event")]
        public List<String> Event { get; set; }


    }
}