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
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace SanteDB.Matcher.Configuration
{

    /// <summary>
    /// Represents a configured match vector
    /// </summary>
    [XmlType(nameof(MatchVector), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchVector))]
    public class MatchVector
    {

        private double? m_m = null;
        private double? m_u = null;
        private double? m_weightM = null;
        private double? m_weightN = null;

        /// <summary>
        /// The identifier for the vector
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// When condition on a match vector
        /// </summary>
        [XmlArray("when"), XmlArrayItem("vector"), JsonProperty("when")]
        public List<MatchVectorWhenCondition> When { get; set; }

        /// <summary>
        /// Gets or sets the match weight
        /// </summary>
        [XmlAttribute("m"), JsonProperty("m")]
        public double M {
            get => this.m_m.GetValueOrDefault();
            set => this.m_m = value;
        }

        /// <summary>
        /// Gets or sets the unmatch weight (penalty)
        /// </summary>
        [XmlAttribute("u"), JsonProperty("u")]
        public double U {
            get => this.m_u.GetValueOrDefault();
            set => this.m_u = value;
        }

        /// <summary>
        /// Gets or sets the match weight
        /// </summary>
        [XmlAttribute("matchWeight"), JsonProperty("matchWeight")]
        public double MatchWeight
        {
            get => this.m_weightM.GetValueOrDefault();
            set => this.m_weightM = value;
        }

        /// <summary>
        /// Gets or sets the unmatch weight (penalty)
        /// </summary>
        [XmlAttribute("nonMatchWeight"), JsonProperty("nonMatchWeight")]
        public double NonMatchWeight {
            get => this.m_weightN.GetValueOrDefault();
            set => this.m_weightN = value;
        }

        /// <summary>
        /// Gets or sets the property 
        /// </summary>
        [XmlAttribute("property"), JsonProperty("property")]
        public List<string> Property { get; set; }

        /// <summary>
        /// When null what should happen?
        /// </summary>
        [XmlAttribute("whenNull"), JsonProperty("whenNull")]
        public MatchVectorNullBehavior WhenNull { get; set; }

        /// <summary>
        /// The match has to be executed, if the inbound object does not have the property (is null)
        /// then the entire matching process stops.
        /// </summary>
        [XmlAttribute("required"), JsonProperty("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        [XmlElement("assert"), JsonProperty("assert")]
        public MatchVectorAssertion Assertion { get; set; }

        /// <summary>
        /// Gets or sets the algorithm for partially measuring the value
        /// </summary>
        [XmlElement("partialWeight"), JsonProperty("partialWeight")]
        public MatchTransform Measure { get; set; }

        /// <summary>
        /// Initializes the probability parameters on this class instance
        /// </summary>
        public void Initialize()
        {
            // Step 1 : Are we missing weights?
            if(!this.m_weightM.HasValue)
            {
                // U must be set
                if (!this.m_u.HasValue || !this.m_m.HasValue) throw new InvalidOperationException("U and M variables must be set");

                this.m_weightM = (this.m_m.Value / this.m_u.Value).Ln() / (2.0d).Ln();
            }
            if(!this.m_weightN.HasValue)
            {
                // U must be set
                if (!this.m_u.HasValue || !this.m_m.HasValue) throw new InvalidOperationException("U and M variables must be set");

                this.m_weightN = ((1 - this.m_m.Value) / (1 - this.m_u.Value)).Ln() / (2.0d).Ln();
            }
        }

        /// <summary>
        /// Represent this vector as a string
        /// </summary>
        public override string ToString() => $"VECTOR: {this.Property} ({this.Assertion}) [match: {this.MatchWeight}, non: {this.NonMatchWeight}, u: {this.U}, m: {this.M}, measure: {this.Measure}]";
    }
}