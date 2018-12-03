using SanteDB.Core;
using SanteDB.Core.Applets.Services;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using SanteDB.Matcher.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Services
{
    /// <summary>
    /// Applet match configuration provider loads match configurations from available applets
    /// </summary>
    public sealed class AppletMatchConfigurationProvider : IRecordMatchingConfigurationService
    {

        // Configuration cache
        private readonly Dictionary<String, IRecordMatchingConfiguration> m_configurationCache = new Dictionary<string, IRecordMatchingConfiguration>();

        // Get the tracer
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(AppletMatchConfigurationProvider));

        /// <summary>
        /// Applet Based Match Configuration Provider
        /// </summary>
        public string ServiceName => "SanteMatch Applet XML Match Configuration";

        /// <summary>
        /// Get the specified configuration name
        /// </summary>
        /// <param name="name">The configuratio name</param>
        /// <returns>The matching configuration</returns>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
            if (this.m_configurationCache.TryGetValue(name, out IRecordMatchingConfiguration retVal))
            {
                var amgr = ApplicationServiceContext.Current.GetService<IAppletManagerService>()?.Applets.SelectMany(a => a.Assets).Where(o => o.Name == $"matching/{name}.xml").FirstOrDefault();

                try
                {
                    this.m_tracer.TraceInfo("Will load {0}..", amgr.ToString());

                    using (var ms = new MemoryStream(amgr.Content as byte[]))
                        retVal = MatchConfiguration.Load(ms);

                    lock (this.m_configurationCache)
                        if (!this.m_configurationCache.ContainsKey(name))
                            this.m_configurationCache.Add(name, retVal);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error loading match config {0} : {1}", name, e.ToString());
                    throw;
                }
            }
            return retVal;
        }
    }
}
