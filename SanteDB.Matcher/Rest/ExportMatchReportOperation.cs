using RestSrvr;
using SanteDB.Core.Interop;
using SanteDB.Core.Matching;
using SanteDB.Core.Model.Query;
using SanteDB.Matcher.Definition;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace SanteDB.Matcher.Rest
{
    /// <summary>
    /// Export matching report operation
    /// </summary>
    public class ExportMatchReportOperation : IApiChildResourceHandler
    {
        // Transform
        private readonly XslCompiledTransform m_transform = new XslCompiledTransform();

        // Match configuration
        private readonly IRecordMatchingConfigurationService m_matchConfiguration;

        /// <summary>
        /// Match report operation
        /// </summary>
        public ExportMatchReportOperation(IRecordMatchingConfigurationService matchConfigurationService = null)
        {
            using (var stream = typeof(ExportMatchReportOperation).Assembly.GetManifestResourceStream("SanteDB.Matcher.Resources.MatchConfiguration.xslt"))
            {
                using (var xr = XmlReader.Create(stream))
                {
                    this.m_transform.Load(xr, new XsltSettings()
                    {
                        EnableScript = true
                    }, null);
                }
            }
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
        public ResourceCapabilityType Capabilities => ResourceCapabilityType.Get;

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
            var configuration = this.m_matchConfiguration?.GetConfiguration(scopingKey.ToString()) as MatchConfiguration;
            if (configuration == null)
            {
                throw new FileNotFoundException($"Match Configuration {scopingKey} not found");
            }

            RestOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            using (var ms = new MemoryStream())
            {
                configuration.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                // Now export
                using (var xr = XmlReader.Create(ms))
                {
                    using (var xw = XmlWriter.Create(RestOperationContext.Current.OutgoingResponse.OutputStream))
                    {
                        this.m_transform.Transform(xr, xw);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Query the object
        /// </summary>

        public IEnumerable<object> Query(Type scopingType, object scopingKey, NameValueCollection filter, int offset, int count, out int totalCount)
        {
            throw new NotSupportedException();
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