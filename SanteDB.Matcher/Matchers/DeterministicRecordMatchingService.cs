﻿/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
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
 * User: justin
 * Date: 2018-12-25
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SanteDB.Core.Services;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents a deterministic record matching service
    /// </summary>
    [ServiceProvider("SanteMatch Deterministic Matcher")]
    public class DeterministicRecordMatchingService : BaseRecordMatchingService
    {

        /// <summary>
        /// Gets the service name
        /// </summary>
        public override string ServiceName => "SanteMatch Deterministic Matcher";

        /// <summary>
        /// Classify the specified inputs
        /// </summary>
        /// <remarks>This particular record matching service only uses the blocking portion of configuration so all blocked records are considered matches</remarks>
        public override IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName)
        {
            return blocks.Select(o => new MatchResult<T>(o, 1.0, RecordMatchClassification.Match));
        }

        /// <summary>
        /// Performs a block and match operation
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName)
        {
            return this.Classify(input, this.Block(input, configurationName), configurationName);
        }
    }
}
