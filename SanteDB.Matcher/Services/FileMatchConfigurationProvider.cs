/*
 *
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2020-1-1
 */
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SanteDB.Matcher.Configuration;

namespace SanteDB.Matcher.Services
{
    /// <summary>
    /// Represents a configuration provider which is for matching
    /// </summary>
    public class FileMatchConfigurationProvider : IRecordMatchingConfigurationService
    {

        /// <summary>
        /// Name and record matching configuration
        /// </summary>
        private ConcurrentDictionary<String, dynamic> m_matchConfigurations;

        // Gets the configuration
        private FileMatchConfigurationSection m_configuration = ApplicationServiceContext.Current.GetService<IConfigurationManager>().GetSection<FileMatchConfigurationSection>();

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(FileMatchConfigurationProvider));

        /// <summary>
        /// Gets the configurations known to this configuration provider
        /// </summary>
        public IEnumerable<string> Configurations => this.m_matchConfigurations.Values.Select(o => o.Configuration.Name).OfType<String>();

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "File Based Match Configuration Provider";

        /// <summary>
        /// Process the file directory 
        /// </summary>
        public FileMatchConfigurationProvider()
        {
            // When application has started
            ApplicationServiceContext.Current.Started += (o, e) =>
            {
                try
                {
                    if (this.m_configuration == null) return;
                    this.m_matchConfigurations = new ConcurrentDictionary<string, dynamic>();
                    this.m_tracer.TraceInfo("Loading match configurations...");
                    foreach (var configDir in this.m_configuration.FilePath)
                    {
                        if (!Directory.Exists(configDir.Path))
                            this.m_tracer.TraceWarning("Skipping {0} because it doesn't exist!", configDir.Path);
                        else
                            foreach (var fileName in Directory.GetFiles(configDir.Path, "*.xml"))
                            {
                                this.m_tracer.TraceInfo("Attempting load of {0}", fileName);
                                try
                                {
                                    using (var fs = System.IO.File.OpenRead(fileName))
                                    {
                                        var config = MatchConfiguration.Load(fs);
                                        this.m_matchConfigurations.TryAdd(config.Name, new
                                        {
                                            OriginalFilePath = fileName,
                                            Configuration = config
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.m_tracer.TraceWarning("Could not load {0} - SKIPPING - {1}", fileName, ex.Message);
                                }
                            }
                    }
                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Could not fully load configuration for matching : {0}", ex);
                }
            };
        }

        /// <summary>
        /// Get the specified configuration
        /// </summary>
        /// <param name="name">The name of the configuration</param>
        /// <returns>The loaded configuration</returns>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
#if DEBUG
            if (this.m_matchConfigurations.TryGetValue(name, out dynamic configData))
                using (var fs = System.IO.File.OpenRead(configData.OriginalFilePath))
                    return MatchConfiguration.Load(fs);
#else
            if (this.m_matchConfigurations.TryGetValue(name, out dynamic configData))
                return configData.Configuration as IRecordMatchingConfiguration;
#endif
            else return null;
        }

        /// <summary>
        /// Save configuration to the file system
        /// </summary>
        /// <param name="configuration">The configuration to be saved</param>
        /// <returns>The updated configuration</returns>
        public IRecordMatchingConfiguration SaveConfiguration(IRecordMatchingConfiguration configuration)
        {
            if (!this.m_matchConfigurations.TryGetValue(configuration.Name, out dynamic configData))
            {
                var savePath = this.m_configuration.FilePath.FirstOrDefault(o => !o.ReadOnly);
                if (savePath == null)
                    throw new InvalidOperationException("Cannot find a read/write configuration path");

                configData = new
                {
                    Configuration = configuration,
                    OriginalFilePath = Path.ChangeExtension(Path.Combine(savePath.Path, Guid.NewGuid().ToString()), "xml")
                };

                if (!this.m_matchConfigurations.TryAdd(configuration.Name, configData))
                    throw new InvalidOperationException("Storing configuration has failed");
            }

            // Open for writing and write the configuration
            try
            {
                using (var fs = System.IO.File.Create(configData.OriginalFilePath))
                {
                    if (configuration is MatchConfiguration)
                        (configuration as MatchConfiguration).Save(fs);
                    else if (configuration is MatchConfigurationCollection)
                        (configuration as MatchConfigurationCollection).Save(fs);
                }
                return configData.Configuration;
            }
            catch (Exception e)
            {
                throw new Exception($"Error while saving match configuration {configuration.Name}", e);
            }
        }
    }
}
