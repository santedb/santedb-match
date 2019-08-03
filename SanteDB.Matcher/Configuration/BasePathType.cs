using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Represents the base path types
    /// </summary>
    [XmlType(nameof(BasePathType), Namespace = "http://santedb.org/configuration")]
    public enum BasePathType
    {
        /// <summary>
        /// The base path points to an assembly
        /// </summary>
        [XmlEnum("assembly")]
        Assembly,
        /// <summary>
        /// The base path points to a custom loader
        /// </summary>
        [XmlEnum("custom")]
        Custom

    }
}