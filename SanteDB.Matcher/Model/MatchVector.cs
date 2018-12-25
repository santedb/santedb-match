/*
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
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{

    /// <summary>
    /// Represents a configured match vector
    /// </summary>
    [XmlType(nameof(MatchVector), Namespace = "http://santedb.org/matcher")]
    public class MatchVector
    {

        private double? m_m = null;
        private double? m_u = null;
        private double? m_weightM = null;
        private double? m_weightN = null;

        /// <summary>
        /// Gets or sets the match weight
        /// </summary>
        [XmlAttribute("m")]
        public double M {
            get => this.m_m.GetValueOrDefault();
            set => this.m_m = value;
        }

        /// <summary>
        /// Gets or sets the unmatch weight (penalty)
        /// </summary>
        [XmlAttribute("u")]
        public double U {
            get => this.m_u.GetValueOrDefault();
            set => this.m_u = value;
        }

        /// <summary>
        /// Gets or sets the match weight
        /// </summary>
        [XmlAttribute("matchWeight")]
        public double MatchWeight
        {
            get => this.m_weightM.GetValueOrDefault();
            set => this.m_weightM = value;
        }

        /// <summary>
        /// Gets or sets the unmatch weight (penalty)
        /// </summary>
        [XmlAttribute("nonMatchWeight")]
        public double NonMatchWeight {
            get => this.m_weightN.GetValueOrDefault();
            set => this.m_weightN = value;
        }

        /// <summary>
        /// Gets or sets the property 
        /// </summary>
        [XmlAttribute("property")]
        public string Property { get; set; }

        /// <summary>
        /// When null what should happen?
        /// </summary>
        [XmlAttribute("whenNull")]
        public MatchVectorNullBehavior WhenNull { get; set; }

        /// <summary>
        /// The match has to be executed, if the inbound object does not have the property (is null)
        /// then the entire matching process stops.
        /// </summary>
        [XmlAttribute("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        [XmlElement("assert")]
        public MatchVectorAssertion Assertion { get; set; }

        /// <summary>
        /// Gets or sets the algorithm for partially measuring the value
        /// </summary>
        [XmlElement("partialWeight")]
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
    }
}