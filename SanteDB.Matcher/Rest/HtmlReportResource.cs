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
 * Date: 2021-10-28
 */
using Newtonsoft.Json;
using RestSrvr;
using SanteDB.Core.Interop;
using SanteDB.Core.Matching;
using SanteDB.Core.Model.Query;
using SanteDB.Matcher.Definition;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace SanteDB.Matcher.Rest
{
    /// <summary>
    /// Export matching report operation
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HtmlReportResource : IApiChildResourceHandler
    {
        // Transform
        private XslCompiledTransform m_transform = null;

        // Match configuration
        private readonly IRecordMatchingConfigurationService m_matchConfiguration;

        /// <summary>
        /// Match report operation
        /// </summary>
        public HtmlReportResource(IRecordMatchingConfigurationService matchConfigurationService = null)
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
        public string Name => "html";

        /// <summary>
        /// Get the property type
        /// </summary>
        public Type PropertyType => typeof(Stream);

        /// <summary>
        /// Capabilities
        /// </summary>
        public ResourceCapabilityType Capabilities => ResourceCapabilityType.Search;

        /// <summary>
        /// Add not supported
        /// </summary>
        public object Add(Type scopingType, object scopingKey, object item)
        {
            throw new NotSupportedException();
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
            if (this.m_transform == null)
            {
                this.m_transform = new XslCompiledTransform();
                using (var stream = typeof(HtmlReportResource).Assembly.GetManifestResourceStream("SanteDB.Matcher.Resources.MatchConfiguration.xslt"))
                {
                    using (var xr = XmlReader.Create(stream))
                    {
                        this.m_transform.Load(xr, new XsltSettings()
                        {
                            EnableScript = true
                        }, null);
                    }
                }
            }
            var configuration = this.m_matchConfiguration?.GetConfiguration(scopingKey.ToString()) as MatchConfiguration;
            if (configuration == null)
            {
                throw new FileNotFoundException($"Match Configuration {scopingKey} not found");
            }

            RestOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            RestOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", $"attachment; filename=\"{scopingKey}.htm\"");
            using (var ms = new MemoryStream())
            {
                // Save configuration to XML
                configuration.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                // Now export
                using (var xr = XmlReader.Create(ms))
                {
                    using (var xw = XmlWriter.Create(RestOperationContext.Current.OutgoingResponse.OutputStream))
                    {
                        var args = new XsltArgumentList();
                        args.AddParam("jsonConfig", "http://santedb.org/matcher", JsonConvert.SerializeObject((object)configuration));
                        this.m_transform.Transform(xr, args, xw);
                    }
                }
            }
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