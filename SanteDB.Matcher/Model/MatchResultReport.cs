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
using Newtonsoft.Json;
using SanteDB.Core.Matching;
using SanteDB.Core.Model;
using SanteDB.Matcher.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a MatchResultReport which encapsulates a MatchResult
    /// </summary>
    [JsonObject, XmlType(nameof(MatchResultReport), Namespace = "http://santedb.org/matcher")]
    public class MatchResultReport : IdentifiedData
    {
        /// <summary>
        /// Default ctor for serialization
        /// </summary>
        public MatchResultReport()
        {
        }

        /// <summary>
        /// Creates a new match result report from the specified match data
        /// </summary>
        public MatchResultReport(MatchResult match)
        {
            this.Score = match.Score;
            this.Strength = match.Strength;
            this.Record = match.Record.Key.GetValueOrDefault();
            this.Classification = match.Classification;
            this.Vectors = match.Vectors.Select(o => new VectorResultReport(o)).ToList();
            this.Method = match.Method;
            this.ConfigurationName = match.Configuration.Id;
        }

        /// <summary>
        /// Gets the score
        /// </summary>
        [XmlElement("score"), JsonProperty("score")]
        public double Score { get; set; }

        /// <summary>
        /// Gets the strength of the match
        /// </summary>
        [XmlElement("strength"), JsonProperty("strength")]
        public double Strength { get; set; }

        /// <summary>
        /// Gets the record
        /// </summary>
        [XmlElement("record"), JsonProperty("record")]
        public Guid Record { get; set; }

        /// <summary>
        /// Gets the classification
        /// </summary>
        [XmlElement("classification"), JsonProperty("classification")]
        public RecordMatchClassification Classification { get; set; }

        /// <summary>
        /// Gets or sets the properties that matched and their score
        /// </summary>
        [XmlArray("vectors"), XmlArrayItem("v"), JsonProperty("vectors")]
        public List<VectorResultReport> Vectors { get; set; }

        /// <summary>
        /// The method of match
        /// </summary>
        [XmlElement("method"), JsonProperty("method")]
        public RecordMatchMethod Method { get; set; }

        /// <summary>
        /// The configuration name
        /// </summary>
        [XmlElement("configuration"), JsonProperty("configuration")]
        public string ConfigurationName { get; set; }

        /// <summary>
        /// Get modified on
        /// </summary>
        public override DateTimeOffset ModifiedOn => DateTimeOffset.Now;
    }
}