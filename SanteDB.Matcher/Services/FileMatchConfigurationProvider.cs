/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2023-6-21
 */
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Matching;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Services;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace SanteDB.Matcher.Services
{
    /// <summary>
    /// Represents a configuration provider which is for matching
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FileMatchConfigurationProvider : IRecordMatchingConfigurationService
    {
        /// <summary>
        /// Configuration cache object
        /// </summary>
        private class ConfigCacheObject
        {
            /// <summary>
            /// Gets or sets the original file path
            /// </summary>
            public String OriginalFilePath { get; set; }

            /// <summary>
            /// Gets or sets the configuration
            /// </summary>
            public dynamic Configuration { get; set; }
        }

        /// <summary>
        /// Name and record matching configuration
        /// </summary>
        private ConcurrentDictionary<String, ConfigCacheObject> m_matchConfigurations;

        // Gets the configuration
        private FileMatchConfigurationSection m_configuration = ApplicationServiceContext.Current.GetService<IConfigurationManager>().GetSection<FileMatchConfigurationSection>();

        // The policy enforcement service
        private IPolicyEnforcementService m_pepService;

        // Localization service
        private ILocalizationService m_localizationService;

        // Tracer
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(FileMatchConfigurationProvider));

        /// <summary>
        /// Gets the configurations known to this configuration provider
        /// </summary>
        public IEnumerable<IRecordMatchingConfiguration> Configurations => this.m_matchConfigurations.Values.Select(o => o.Configuration).OfType<IRecordMatchingConfiguration>();

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "File Based Match Configuration Provider";

        /// <summary>
        /// Process the file directory
        /// </summary>
        public FileMatchConfigurationProvider(IConfigurationManager configurationManager, IPolicyEnforcementService pepService, ILocalizationService localizationService)
        {
            this.m_pepService = pepService;
            this.m_configuration = configurationManager.GetSection<FileMatchConfigurationSection>();
            this.m_localizationService = localizationService;

            foreach (var fp in this.m_configuration.FilePath.Where(o => !o.ReadOnly))
            {
                if (!Directory.Exists(fp.Path))
                {
                    Directory.CreateDirectory(fp.Path);
                }
            }
            // When application has started
            try
            {
                if (this.m_configuration == null)
                {
                    this.m_matchConfigurations = new ConcurrentDictionary<string, ConfigCacheObject>();
                    return;
                }

                this.m_matchConfigurations = new ConcurrentDictionary<string, ConfigCacheObject>();
                this.m_tracer.TraceInfo("Loading match configurations...");
                foreach (var configDir in this.m_configuration.FilePath)
                {
                    if (!Path.IsPathRooted(configDir.Path))
                    {
                        configDir.Path = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), configDir.Path);
                    }

                    if (!Directory.Exists(configDir.Path))
                    {
                        this.m_tracer.TraceWarning("Skipping {0} because it doesn't exist!", configDir.Path);
                    }
                    else
                    {
                        foreach (var fileName in Directory.GetFiles(configDir.Path, "*.xml"))
                        {
                            this.m_tracer.TraceInfo("Attempting load of {0}", fileName);
                            try
                            {
                                MatchConfiguration config = null;
                                using (var fs = System.IO.File.OpenRead(fileName))
                                {
                                    config = MatchConfiguration.Load(fs);
                                }

                                var originalPath = fileName;
                                if (!Guid.TryParse(Path.GetFileNameWithoutExtension(fileName), out Guid uuid) || uuid != config.Uuid) // Migrate the config
                                {
                                    originalPath = Path.Combine(configDir.Path, $"{config.Uuid}.xml");
                                    File.Move(fileName, originalPath);
                                    using (var fs = File.Create(originalPath))
                                    {
                                        config.Save(fs);
                                    }
                                }

                                this.m_matchConfigurations.TryAdd(config.Id, new ConfigCacheObject()
                                {
                                    OriginalFilePath = originalPath,
                                    Configuration = config
                                });
                            }
                            catch (Exception ex)
                            {
                                this.m_tracer.TraceWarning("Could not load {0} - SKIPPING - {1}", fileName, ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Could not fully load configuration for matching : {0}", ex);
            }
        }

        /// <summary>
        /// Get the specified configuration
        /// </summary>
        /// <param name="name">The name of the configuration</param>
        /// <returns>The loaded configuration</returns>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
            if (this.m_matchConfigurations.TryGetValue(name, out ConfigCacheObject configData))
            {
                if (this.m_configuration.CacheFiles)
                {
                    return configData.Configuration as IRecordMatchingConfiguration;
                }
                else
                {
                    using (var fs = System.IO.File.OpenRead(configData.OriginalFilePath))
                    {
                        return MatchConfiguration.Load(fs);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Save configuration to the file system
        /// </summary>
        /// <param name="configuration">The configuration to be saved</param>
        /// <returns>The updated configuration</returns>
        public IRecordMatchingConfiguration SaveConfiguration(IRecordMatchingConfiguration configuration)
        {

            if (AuthenticationContext.Current.Principal != AuthenticationContext.SystemPrincipal && ApplicationServiceContext.Current.HostType != SanteDBHostType.Server)
            {
                this.m_pepService.Demand(PermissionPolicyIdentifiers.AlterMatchConfiguration);
            }

            if (!this.m_matchConfigurations.TryGetValue(configuration.Id, out ConfigCacheObject configData))
            {
                // Lookup by UUID

                if (this.m_matchConfigurations.Any(o => o.Value.Configuration.Uuid == configuration.Uuid))
                {
                    throw new InvalidOperationException(this.m_localizationService.GetString("error.server.core.duplicateKey", new { id = configuration.Uuid }));
                }
                else
                {
                    var savePath = this.m_configuration.FilePath.FirstOrDefault(o => !o.ReadOnly);
                    if (savePath == null)
                    {
                        throw new InvalidOperationException("Cannot find a read/write configuration path");
                    }

                    // Set select metadata
                    if (configuration is MatchConfiguration mci)
                    {
                        mci.Metadata.CreationTime = DateTime.Now;
                        mci.Metadata.CreatedBy = AuthenticationContext.Current.Principal.Identity.Name;
                        mci.Metadata.Version = 1;
                    }
                    else
                    {
                        configuration.Metadata = new MatchConfigurationMetadata(configuration.Metadata)
                        {
                            CreationTime = DateTime.Now,
                            CreatedBy = AuthenticationContext.Current.Principal.Identity.Name
                        };
                    }

                    configData = new ConfigCacheObject()
                    {
                        Configuration = configuration,
                        OriginalFilePath = Path.ChangeExtension(Path.Combine(savePath.Path, configuration.Uuid.ToString()), "xml")
                    };

                    if (!this.m_matchConfigurations.TryAdd(configuration.Id, configData))
                    {
                        throw new InvalidOperationException("Storing configuration has failed");
                    }
                }
            }
            else if (configuration is MatchConfiguration mc)
            {
                mc.Metadata.UpdatedTime = DateTime.Now;
                mc.Metadata.UpdatedBy = AuthenticationContext.Current.Principal.Identity.Name;
                mc.Metadata.Version++;
            }

            // Open for writing and write the configuration
            try
            {
                using (var fs = System.IO.File.Create(configData.OriginalFilePath))
                {
                    if (configuration is MatchConfiguration mc)
                    {
                        // is the user changing the state
                        if (mc.Metadata.Status != configData.Configuration.Metadata.Status)
                        {
                            this.m_pepService.Demand(PermissionPolicyIdentifiers.ActivateMatchConfiguration);
                        }

                        mc.Save(fs);
                    }
                    else if (configuration is MatchConfigurationCollection mcc)
                    {
                        mcc.Save(fs);
                    }
                }

                configData.Configuration = configuration;

                return configData.Configuration;
            }
            catch (Exception e)
            {
                throw new Exception($"Error while saving match configuration {configuration.Id}", e);
            }
        }

        /// <summary>
        /// Delete the specified configuration
        /// </summary>
        public IRecordMatchingConfiguration DeleteConfiguration(string name)
        {
            this.m_pepService.Demand(PermissionPolicyIdentifiers.AlterMatchConfiguration);

            if (this.m_matchConfigurations.TryRemove(name, out ConfigCacheObject configData))
            {
                try
                {
                    File.Delete(configData.OriginalFilePath);
                    return configData.Configuration;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error removing {0} - {1}", name, e.Message);
                    throw new IOException($"Error removing {name}", e);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Could not find match configuration {name}");
            }
        }

        /// <summary>
        /// Get all configurations from this provider
        /// </summary>
        public IEnumerable<IRecordMatchingConfiguration> GetConfigurations<T>(Expression<Func<IRecordMatchingConfiguration, bool>> filter)
        {
            return this.m_matchConfigurations.Values.Select(o => o.Configuration).OfType<IRecordMatchingConfiguration>().Where(o => o.AppliesTo.Contains(typeof(T))).Where(filter.Compile());
        }
    }
}