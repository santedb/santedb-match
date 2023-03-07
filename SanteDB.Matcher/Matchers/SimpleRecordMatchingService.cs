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
 * Date: 2022-5-30
 */
using SanteDB.Core;
using SanteDB.Core.Matching;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents a deterministic record matching service
    /// </summary>
    [ServiceProvider("SanteMatch Deterministic Matcher")]
    public class SimpleRecordMatchingService : BaseRecordMatchingService
    {
        /// <summary>
        /// Gets the service name
        /// </summary>
        public override string ServiceName => "SanteMatch Simple Matcher";

        /// <summary>
        /// Classify the specified inputs
        /// </summary>
        /// <remarks>This particular record matching service only uses the blocking portion of configuration so all blocked records are considered matches</remarks>
        public override IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName, IRecordMatchingDiagnosticSession collector = null)
        {
            try
            {
                collector?.LogStartStage("scoring");

                // Get configuration if specified
                var configService = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>();
                if (configService == null)
                {
                    throw new InvalidOperationException("Cannot find configuration service for matching");
                }

                var config = configService.GetConfiguration(configurationName);
                if (config == null)
                {
                    throw new KeyNotFoundException($"Cannot find configuration named {configurationName}");
                }

                return blocks.Select(o => new MatchResult<T>(o, 1.0, 1.0, config, RecordMatchClassification.Match, RecordMatchMethod.Simple, null));
            }
            finally
            {
                collector?.LogEndStage();
            }
        }

        /// <summary>
        /// Performs a block and match operation
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName, IEnumerable<Guid> ignoreList, IRecordMatchingDiagnosticSession collector = null)
        {
            try
            {
                collector?.LogStart(configurationName);
                return this.Classify(input, this.Block(input, configurationName, ignoreList), configurationName, collector);
            }
            finally
            {
                collector?.LogEnd();
            }
        }
    }
}