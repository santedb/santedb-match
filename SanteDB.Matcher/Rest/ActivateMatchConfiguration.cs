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
using SanteDB.Core.Interop;
using SanteDB.Core.Matching;
using SanteDB.Core.Model.Parameters;
using SanteDB.Matcher.Definition;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SanteDB.Matcher.Rest
{
    /// <summary>
    /// Export matching report operation
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ActivateMatchOperation : IApiChildOperation
    {
        // Match configuration
        private readonly IRecordMatchingConfigurationService m_matchConfiguration;

        /// <summary>
        /// Match report operation
        /// </summary>
        public ActivateMatchOperation(IRecordMatchingConfigurationService matchConfigurationService)
        {
            this.m_matchConfiguration = matchConfigurationService;
        }

        /// <summary>
        /// Gets the binding
        /// </summary>
        public ChildObjectScopeBinding ScopeBinding => ChildObjectScopeBinding.Instance;

        /// <summary>
        /// Parent types
        /// </summary>
        public Type[] ParentTypes => new Type[] { typeof(IRecordMatchingConfiguration) };

        /// <summary>
        /// Get the name of this operation
        /// </summary>
        public string Name => "state";

        /// <summary>
        /// Get the property type
        /// </summary>
        public Type PropertyType => typeof(Stream);

        /// <summary>
        /// Invoke the specified operation
        /// </summary>
        public object Invoke(Type scopingType, object scopingKey, ParameterCollection parameters)
        {
            var configuration = this.m_matchConfiguration.GetConfiguration(scopingKey.ToString());
            if (configuration == null || !(configuration is MatchConfiguration matchConfiguration))
            {
                throw new KeyNotFoundException($"Match configuration {scopingKey} not found");
            }

            // Get status
            if (parameters.TryGet<bool>("state", out bool state) && state)
            {
                matchConfiguration.Metadata.Status = MatchConfigurationStatus.Active;
            }
            else
            {
                matchConfiguration.Metadata.Status = MatchConfigurationStatus.Inactive;
            }
            return this.m_matchConfiguration.SaveConfiguration(matchConfiguration);
        }
    }
}