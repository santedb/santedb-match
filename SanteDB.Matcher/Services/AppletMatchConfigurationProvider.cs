/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using SanteDB.Core;
using SanteDB.Core.Applets.Services;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using SanteDB.Core.Matching;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

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
        /// Save configuration to the source
        /// </summary>
        public IRecordMatchingConfiguration SaveConfiguration(IRecordMatchingConfiguration configuration)
        {
            throw new NotSupportedException("This service does not support saving match configurations");
        }

        /// <summary>
        /// Configurations
        /// </summary>
        public IEnumerable<IRecordMatchingConfiguration> Configurations => this.m_configurationCache.Values;

        /// <summary>
        /// Get the specified configuration name
        /// </summary>
        /// <param name="name">The configuratio name</param>
        /// <returns>The matching configuration</returns>
        public IRecordMatchingConfiguration GetConfiguration(string name)
        {
            if (this.m_configurationCache.TryGetValue(name, out IRecordMatchingConfiguration retVal))
            {
                var solutions = ApplicationServiceContext.Current.GetService<IAppletSolutionManagerService>()?.Solutions.Select(o=>o.Meta.Id).ToList();
                solutions.Add(String.Empty); // Include the default solution

                // Solution 
                foreach (var sln in solutions)
                {
                    var amgr = ApplicationServiceContext.Current.GetService<IAppletSolutionManagerService>()?.GetApplets(sln).SelectMany(a => a.Assets).Where(o => o.Name == $"matching/{name}.xml").FirstOrDefault();

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
            }
            return retVal;
        }

        /// <summary>
        /// Delete configuration
        /// </summary>
        public IRecordMatchingConfiguration DeleteConfiguration(string name)
        {
            throw new NotSupportedException("This service does not support saving match configurations");
        }

        /// <summary>
        /// Set metadata
        /// </summary>
        public IRecordMatchingConfiguration SetMetadata(string name, IRecordMatchingConfigurationMetadata metadata)
        {
            throw new NotSupportedException("This service does not support saving match configurations");
        }

        /// <summary>
        /// Get configurations
        /// </summary>
        public IEnumerable<IRecordMatchingConfiguration> GetConfigurations<T>(Expression<Func<IRecordMatchingConfiguration, bool>> filter)
        {
            return this.m_configurationCache.Values.Where(o => o.AppliesTo.Contains(typeof(T))).Where(filter.Compile());
        }
    }
}
