using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration.File
{
    /// <summary>
    /// Represents a file path configuration
    /// </summary>
    [XmlType(nameof(FilePathConfiguration), Namespace = "http://santedb.org/configuration")]
    [JsonObject(nameof(FilePathConfiguration))]
    public class FilePathConfiguration
    {

        /// <summary>
        /// True if the path should be treated as readonly
        /// </summary>
        [XmlAttribute("readonly")]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [XmlText, JsonProperty("value")]
        public string Path { get; set; }
    }
}