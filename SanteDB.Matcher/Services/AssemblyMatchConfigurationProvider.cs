/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-27
 */
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Matching;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SanteDB.Matcher.Services
{
    /// <summary>
    /// File based match configuration provider
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class AssemblyMatchConfigurationProvider : IRecordMatchingConfigurationService
    {
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(AssemblyMatchConfigurationProvider));

        // Configurations
        private List<IRecordMatchingConfiguration> m_configurations = new List<IRecordMatchingConfiguration>();

        /// <summary>
        /// CTOR will load configuration
        /// </summary>
        public AssemblyMatchConfigurationProvider()
        {
            var config = ApplicationServiceContext.Current.GetService<IConfigurationManager>().GetSection<AssemblyMatchConfigurationSection>();
            try
            {
                foreach (var asmRef in config.Assemblies)
                {
                    this.m_tracer.TraceInfo("Loading configurations from {0}", asmRef);
                    var asm = Assembly.Load(new AssemblyName(asmRef));
                    if (asm == null)
                        throw new FileNotFoundException($"Assembly {asmRef} could not be found");
                    // Get embedded resource names
                    foreach (var name in asm.GetManifestResourceNames().Where(o => o.EndsWith(".xml")))
                    {
                        this.m_tracer.TraceVerbose("Attempting load of {0}...", name);
                        using (var s = asm.GetManifestResourceStream(name))
                        {
                            try
                            {
                                var conf = MatchConfigurationCollection.Load(s);
                                this.m_configurations.Add(conf);
                                this.m_tracer.TraceInfo("Loaded match configuration collection {0}", conf.Id);
                            }
                            catch
                            {
                                try
                                {
                                    var conf = MatchConfiguration.Load(s);
                                    this.m_configurations.Add(conf);
                                    this.m_tracer.TraceInfo("Loaded match configuration {0}", conf.Id);
                                }
                                catch { }
                            }
                        }
                    }
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
        public string ServiceName => "Assembly Match Configuration";

        /// <summary>
        /// Gets all configurations
        /// </summary>
        public IEnumerable<IRecordMatchingConfiguration> Configurations => this.m_configurations;

        /// <summary>
        /// Delete configurations
        /// </summary>
        public IRecordMatchingConfiguration DeleteConfiguration(string name)
        {
            throw new NotSupportedException("Saving to assemblies is not supported");
        }

        /// <summary>
        /// Gets the specified configuration name
        /// </summary>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
            return this.m_configurations.FirstOrDefault(o => o.Id == name);
        }

        /// <summary>
        /// Get all configurations of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public IEnumerable<IRecordMatchingConfiguration> GetConfigurations<T>(Expression<Func<IRecordMatchingConfiguration, bool>> filter)
        {
            return this.m_configurations.Where(o => o.AppliesTo.Contains(typeof(T))).Where(filter.Compile());
        }

        /// <summary>
        /// Save configuration to the specified stream
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IRecordMatchingConfiguration SaveConfiguration(IRecordMatchingConfiguration config)
        {
            throw new NotSupportedException("Saving to assemblies is not supported");
        }

        /// <summary>
        /// Saving to assemblies not supported
        /// </summary>
        public IRecordMatchingConfiguration SetMetadata(string name, IRecordMatchingConfigurationMetadata metadata)
        {
            throw new NotSupportedException("Saving to assemblies is not supported");
        }
    }
}