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
    /// A configuration section for the file configuraiton provider
    /// </summary>
    [XmlType(nameof(AssemblyMatchConfigurationSection), Namespace = "http://santedb.org/configuration")]
    public class AssemblyMatchConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// Gets the base path from which the file based configuration should be loaded
        /// </summary>
        [XmlArray("assemblies"), XmlArrayItem("add")]
        public List<String> Assemblies { get; set; }


    }
}
