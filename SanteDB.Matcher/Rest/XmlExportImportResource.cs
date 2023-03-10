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
 * Date: 2023-3-10
 */
using RestSrvr;
using SanteDB.Core.Interop;
using SanteDB.Core.Matching;
using SanteDB.Core.Model.Query;
using SanteDB.Matcher.Definition;
using SanteDB.Rest.Common;
using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SanteDB.Matcher.Rest
{
    /// <summary>
    /// Export matching configuration as an XML file
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class XmlExportImportResource : IApiChildResourceHandler
    {

        // Match configuration
        private readonly IRecordMatchingConfigurationService m_matchConfiguration;

        /// <summary>
        /// Match report operation
        /// </summary>
        public XmlExportImportResource(IRecordMatchingConfigurationService matchConfigurationService = null)
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
        public string Name => "xml";

        /// <summary>
        /// Get the property type
        /// </summary>
        public Type PropertyType => typeof(Stream);

        /// <summary>
        /// Capabilities
        /// </summary>
        public ResourceCapabilityType Capabilities => ResourceCapabilityType.Search | ResourceCapabilityType.Create;

        /// <summary>
        /// Add not supported
        /// </summary>
        public object Add(Type scopingType, object scopingKey, object item)
        {
            if (item is Stream stream)
            {
                var configuration = MatchConfiguration.Load(stream);
                RestOperationContext.Current.OutgoingResponse.StatusCode = 201;
                return this.m_matchConfiguration.SaveConfiguration(configuration);
            }
            else
            {
                throw new InvalidOperationException("Cannot process this request");
            }
        }

        /// <summary>
        /// Get the object
        /// </summary>
        public object Get(Type scopingType, object scopingKey, object key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Query the object
        /// </summary>
        public IQueryResultSet Query(Type scopingType, object scopingKey, NameValueCollection filter)
        {
            var configuration = this.m_matchConfiguration?.GetConfiguration(scopingKey.ToString()) as MatchConfiguration;
            if (configuration == null)
            {
                throw new FileNotFoundException($"Match Configuration {scopingKey} not found");
            }

            RestOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            RestOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", $"attachment; filename=\"{scopingKey}.xml\"");
            configuration.Save(RestOperationContext.Current.OutgoingResponse.OutputStream);
            return null;
        }

        /// <summary>
        /// Remove - not supported
        /// </summary>
        public object Remove(Type scopingType, object scopingKey, object key)
        {
            throw new NotSupportedException();
        }
    }
}