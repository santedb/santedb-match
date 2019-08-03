using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Services
{
    /// <summary>
    /// File based match configuration provider
    /// </summary>
    public class StreamMatchConfigurationProvider : IRecordMatchingConfigurationService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(StreamMatchConfigurationProvider));

        // Configurations
        private List<IRecordMatchingConfiguration> m_configurations = new List<IRecordMatchingConfiguration>();

        /// <summary>
        /// CTOR will load configuration
        /// </summary>
        public StreamMatchConfigurationProvider()
        {
            var config = ApplicationServiceContext.Current.GetService<IConfigurationManager>().GetSection<StreamMatchConfigurationSection>();
            try
            {
                this.m_tracer.TraceInfo("Loading configurations from {0}, {1}", config.StreamProviderType, config.StreamProvider);
                switch (config.StreamProviderType)
                {
                    case BasePathType.Assembly:
                        var asm = Assembly.Load(new AssemblyName(config.StreamProvider));
                        if (asm == null)
                            throw new FileNotFoundException($"Assembly {config.StreamProvider} could not be found");
                        // Get embedded resource names
                        foreach (var name in asm.GetManifestResourceNames())
                        {
                            this.m_tracer.TraceVerbose("Attempting load of {0}...", name);
                            using (var s = asm.GetManifestResourceStream(name))
                            {
                                try
                                {
                                    var conf = MatchConfiguration.Load(s);
                                    this.m_configurations.Add(conf);
                                    this.m_tracer.TraceInfo("Loaded match configuration {0}", conf.Name);
                                }
                                catch { }
                            }
                        }
                        break;
                    case BasePathType.Custom:
                        var typ = Type.GetType(config.StreamProvider);
                        if (typ == null)
                            throw new KeyNotFoundException($"Could not find {config.StreamProvider}");
                        var inst = Activator.CreateInstance(typ) as IStreamMatchConfigurationProvider;
                        if (inst == null)
                            throw new InvalidOperationException($"{typ} does not implement IStreamMatchConfigurationProvider");
                        foreach (var s in inst.GetMatchConfigurations())
                        {
                            try
                            {
                                var conf = MatchConfiguration.Load(s);
                                this.m_configurations.Add(conf);
                                this.m_tracer.TraceInfo("Loaded match configuration {0}", conf.Name);
                            }
                            catch { }
                            finally
                            {
                                s.Dispose();
                            }
                        }
                        break;
                }

            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error starting file configuration match provider: {0}", e);
                throw;
            }
        }

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "File Based Match Configuration";

        /// <summary>
        /// Gets all configurations 
        /// </summary>
        public IEnumerable<string> Configurations => this.m_configurations.Select(o => o.Name);

        /// <summary>
        /// Gets the specified configuration name
        /// </summary>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
            return this.m_configurations.FirstOrDefault(o => o.Name == name);
        }
    }
}
