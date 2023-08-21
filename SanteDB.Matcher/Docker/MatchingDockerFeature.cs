/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-5-19
 */
using SanteDB.Core.Configuration;
using SanteDB.Core.Exceptions;
using SanteDB.Docker.Core;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Matchers;
using SanteDB.Matcher.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SanteDB.Matcher.Docker
{
    /// <summary>
    /// Docker feature for matching configuration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MatchingDockerFeature : IDockerFeature
    {

        // Mode setting
        /// <summary>
        /// The mode setting key.
        /// </summary>
        public const string ModeSetting = "MODE";
        /// <summary>
        /// Use the <see cref="SimpleRecordMatchingService"/>.
        /// </summary>
        public const string SimpleMode = "SIMPLE";
        /// <summary>
        /// Use the <see cref="WeightedRecordMatchingService"/>.
        /// </summary>
        public const string WeightedMode = "WEIGHTED";

        /// <summary>
        /// Gets the id of the matching feature
        /// </summary>
        public string Id => "MATCHING";

        /// <summary>
        /// Gets the settings for this feature
        /// </summary>
        public IEnumerable<string> Settings => new String[] { ModeSetting };

        /// <summary>
        /// Configure the feature
        /// </summary>
        public void Configure(SanteDBConfiguration configuration, IDictionary<string, string> settings)
        {
            if (configuration.GetSection<FileMatchConfigurationSection>() == null) // Add configuration for file matching
            {
                configuration.AddSection(DockerFeatureUtils.LoadConfigurationResource<FileMatchConfigurationSection>("SanteDB.Matcher.Docker.MatchingFeature.xml"));
            }

            // Add the file match configuration service
            var serviceConfiguration = configuration.GetSection<ApplicationServiceContextConfigurationSection>().ServiceProviders;
            if (!serviceConfiguration.Any(s => s.Type == typeof(FileMatchConfigurationProvider)))
            {
                serviceConfiguration.Add(new TypeReferenceConfiguration(typeof(FileMatchConfigurationProvider)));
            }

            // Did the hoster specify a matcher they want to use?
            if (settings.TryGetValue(ModeSetting, out string matcher))
            {
                switch (matcher)
                {
                    case SimpleMode:
                        serviceConfiguration.Add(new TypeReferenceConfiguration(typeof(SimpleRecordMatchingService)));
                        break;
                    case WeightedMode:
                        serviceConfiguration.Add(new TypeReferenceConfiguration(typeof(WeightedRecordMatchingService)));
                        break;
                    default:
                        throw new ConfigurationException($"{matcher} is not a valid valud for SDB_MATCHING_MODE. Use either SIMPLE or WEIGHTED", configuration);
                }
            }
            else
            {
                serviceConfiguration.Add(new TypeReferenceConfiguration(typeof(WeightedRecordMatchingService)));
            }
        }
    }
}
