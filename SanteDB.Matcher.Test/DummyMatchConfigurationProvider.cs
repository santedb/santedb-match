using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SanteDB.Matcher.Model;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;

namespace SanteDB.Matcher.Test
{
    /// <summary>
    /// Dummy configuration provider
    /// </summary>
    public class DummyMatchConfigurationProvider : IRecordMatchingConfigurationService
    {

        public string ServiceName => "Fake News Record Matching";

        /// <summary>
        /// All configuration 
        /// </summary>
        public IEnumerable<string> Configurations => this.m_configs.Select(o => o.Name);

        private List<IRecordMatchingConfiguration> m_configs = new List<IRecordMatchingConfiguration>();

        /// <summary>
        /// Dummy match configuration provider
        /// </summary>
        public DummyMatchConfigurationProvider()
        {
            foreach (var n in typeof(DummyMatchConfigurationProvider).Assembly.GetManifestResourceNames().Where(o => o.Contains(".xml")))
                try
                {
                    this.m_configs.Add(MatchConfiguration.Load(typeof(DummyMatchConfigurationProvider).Assembly.GetManifestResourceStream(n)));
                }
                catch { }
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
            return this.m_configs.FirstOrDefault(o => o.Name == name);
        }

        public IRecordMatchingConfiguration SaveConfiguration(IRecordMatchingConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
