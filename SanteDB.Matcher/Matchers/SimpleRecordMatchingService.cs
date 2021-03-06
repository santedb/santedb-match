﻿/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SanteDB.Core.Services;

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
        public override IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName)
        {
            return blocks.Select(o => new MatchResult<T>(o, 1.0, 1.0, RecordMatchClassification.Match, RecordMatchMethod.Deterministic,null));
        }

        /// <summary>
        /// Performs a block and match operation
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName, IEnumerable<Guid> ignoreList)
        {
            return this.Classify(input, this.Block(input, configurationName, ignoreList), configurationName);
        }

    }
}
