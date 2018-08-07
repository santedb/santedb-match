using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents an IMSI based expression filter
    /// </summary>
    [XmlType(nameof(ImsiExpressionFilter), Namespace = "http://santedb.org/matcher")]
    public class ImsiExpressionFilter
    {

        /// <summary>
        /// True if value is null
        /// </summary>
        [XmlAttribute("trueIfNull")]
        public bool TrueIfNull { get; set; }

        /// <summary>
        /// Gets or sets the actual filter
        /// </summary>
        [XmlText]
        public string Filter { get; set; }

    }
}