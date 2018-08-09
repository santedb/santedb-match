using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Weight types for match vectors
    /// </summary>
    [XmlType(nameof(MatchVectorWeightType), Namespace = "http://santedb.org/matcher")]
    public enum MatchVectorWeightType
    {
        /// <summary>
        /// Full weight - The weight is simply added
        /// </summary>
        [XmlEnum("full")]
        Full,
        /// <summary>
        /// Partial weight - The weight is multiplied by how different or how far from the other value the 
        /// value is.
        /// </summary>
        [XmlEnum("partial")]
        Partial
    }
}