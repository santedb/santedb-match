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
using SanteDB.Core.Matching;
using System.Linq.Expressions;

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
        public IEnumerable<IRecordMatchingConfiguration> Configurations => this.m_configs;


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
            return this.m_configs.FirstOrDefault(o => o.Id == name);
        }

        public IRecordMatchingConfiguration SaveConfiguration(IRecordMatchingConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public IRecordMatchingConfiguration DeleteConfiguration(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRecordMatchingConfiguration> GetConfigurations<T>(Expression<Func<IRecordMatchingConfiguration, bool>> filter)
        {
            return this.Configurations.Where(o => o.AppliesTo.Contains(typeof(T))).Where(filter.Compile());
        }
    }
}
