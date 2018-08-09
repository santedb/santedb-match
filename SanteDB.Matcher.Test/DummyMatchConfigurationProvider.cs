using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SanteDB.Matcher.Model;

namespace SanteDB.Matcher.Test
{
    /// <summary>
    /// Dummy configuration provider
    /// </summary>
    public class DummyMatchConfigurationProvider : IRecordMatchingConfigurationService
    {

        private List<IRecordMatchingConfiguration> m_configs = new List<IRecordMatchingConfiguration>();

        /// <summary>
        /// Dummy match configuration provider
        /// </summary>
        public DummyMatchConfigurationProvider()
        {
            foreach (var n in typeof(DummyMatchConfigurationProvider).Assembly.GetManifestResourceNames().Where(o=>o.Contains(".xml")))
                this.m_configs.Add(MatchConfiguration.Load(typeof(DummyMatchConfigurationProvider).Assembly.GetManifestResourceStream(n)));
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
            return this.m_configs.FirstOrDefault(o => o.Name == name);
        }
    }
}
