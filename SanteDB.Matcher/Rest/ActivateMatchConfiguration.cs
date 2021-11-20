using RestSrvr;
using SanteDB.Core.Interop;
using SanteDB.Core.Matching;
using SanteDB.Core.Model.Parameters;
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
                matchConfiguration.Metadata.State = MatchConfigurationStatus.Active;
            }
            else
            {
                matchConfiguration.Metadata.State = MatchConfigurationStatus.Inactive;
            }
            return this.m_matchConfiguration.SaveConfiguration(matchConfiguration);
        }
    }
}