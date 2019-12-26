using SanteDB.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{

    /// <summary>
    /// Stream match configuration provider
    /// </summary>
    public interface IStreamMatchConfigurationProvider
    {

        /// <summary>
        /// Get match configurations
        /// </summary>
        IEnumerable<Stream> GetMatchConfigurations();

    }

    /// <summary>
    /// A configuration section for the file configuraiton provider
    /// </summary>
    [XmlType(nameof(StreamMatchConfigurationSection), Namespace = "http://santedb.org/configuration")]
    public class StreamMatchConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// Gets or sets the base path type
        /// </summary>
        [XmlAttribute("providerType")]
        public BasePathType StreamProviderType { get; set; }

        /// <summary>
        /// Gets the base path from which the file based configuration should be loaded
        /// </summary>
        [XmlElement("providerRef")]
        public String StreamProvider { get; set; }


    }
}
