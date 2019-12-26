using Newtonsoft.Json;
using SanteDB.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration.File
{
    /// <summary>
    /// Configures the file matching configuration
    /// </summary>
    [XmlType(nameof(FileMatchConfigurationSection), Namespace = "http://santedb.org/configuration")]
    [JsonObject(nameof(FileMatchConfigurationSection))]
    public class FileMatchConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// File path
        /// </summary>
        [XmlArray("basePath"), XmlArrayItem("add"), JsonProperty("basePath")]
        public List<FilePathConfiguration> FilePath { get; set; }

    }
}
