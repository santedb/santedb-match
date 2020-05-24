/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using Newtonsoft.Json;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Acts;
using SanteDB.Core.Model.Entities;
using SanteDB.Core.Model.Roles;
using SanteDB.Core.Services;
using SanteDB.Matcher.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a MatchResultReport which encapsulates a MatchResult
    /// </summary>
    [JsonObject, XmlType(nameof(MatchResultReport), Namespace = "http://santedb.org/matcher")]
    public class MatchResultReport
    {

        // The match data
        private MatchResult<IdentifiedData> m_match;

        /// <summary>
        /// Default ctor for serialization
        /// </summary>
        public MatchResultReport()
        {
        }

        /// <summary>
        /// Creates a new match result report from the specified match data
        /// </summary>
        public MatchResultReport(MatchResult<IdentifiedData> match)
        {
            this.m_match = match;
        }

        /// <summary>
        /// Gets the score
        /// </summary>
        [XmlElement("score"), JsonProperty("score")]
        public double Score
        {
            get => this.m_match.Score;
            set { }
        }

        /// <summary>
        /// Gets the confidence that this is a match (the number of vectors that were actually assessed)
        /// </summary>
        [XmlElement("evaluated"), JsonProperty("evaluated")]
        public double EvaluatedVectors
        {
            get => this.m_match.EvaluatedVectors;
            set { }
        }

        /// <summary>
        /// Gets the record
        /// </summary>
        [XmlElement("record"), JsonProperty("record")]
        public IdentifiedData Record
        {
            get => this.m_match.Record;
            set { }
        }

        /// <summary>
        /// Gets the classification 
        /// </summary>
        [XmlElement("classification"), JsonProperty("classification")]
        public RecordMatchClassification Classification
        {
            get => this.m_match.Classification;
            set { }
        }

        /// <summary>
        /// Gets or sets the properties that matched and their score
        /// </summary>
        [XmlArray("vectors"), XmlArrayItem("v"), JsonProperty("vector")]
        public List<VectorResultReport> Vectors
        {
            get => this.m_match.Vectors.Select(o => new VectorResultReport(o)).ToList();
            set { }
        }


    }
}
