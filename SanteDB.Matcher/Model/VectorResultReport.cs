/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using Newtonsoft.Json;
using SanteDB.Core.Model;
using SanteDB.Matcher.Matchers;
using System;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a match assertion
    /// </summary>
    [XmlType(nameof(VectorResultReport), Namespace = "http://santedb.org/matcher"), JsonObject]
    public class VectorResultReport : IdentifiedData
    {

        // True if the object has configuration data
        private bool m_hasConfigurationData = true;

        /// <summary>
        /// Default ctor for serializer
        /// </summary>
        public VectorResultReport()
        {
        }

        /// <summary>
        /// Creates a new result report from the specified result
        /// </summary>
        public VectorResultReport(MatchVector result)
        {
            this.Name = result.Name;
            this.Evaluated = result.Evaluated;

            if (result.Attribute == null)
            {
                this.m_hasConfigurationData = false;
            }
            else
            {
                this.ConfiguredProbability = result.Attribute.M;
                this.ConfiguredWeight = result.Attribute.MatchWeight;
                this.ConfiguredUncertainty = result.Attribute.U;
            }
            this.Score = result.Score;
            this.A = result.A?.ToString();
            this.B = result.B?.ToString();
        }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// True if the assertion was evaluated
        /// </summary>
        [XmlAttribute("evaluated"), JsonProperty("evaluated")]
        public bool Evaluated { get; set; }

        /// <summary>
        /// Gets the configured probability
        /// </summary>
        [XmlAttribute("m"), JsonProperty("m")]
        public double ConfiguredProbability { get; set; }

        /// <summary>
        /// True if serialization of configured weight
        /// </summary>
        public bool ShouldSerializeConfiguredProbability() => this.m_hasConfigurationData;

        /// <summary>
        /// Gets the U value
        /// </summary>
        [XmlAttribute("u"), JsonProperty("u")]
        public double ConfiguredUncertainty { get; set; }

        /// <summary>
        /// True if serialization of configured weight
        /// </summary>
        public bool ShouldSerializeConfiguredUncertainty() => this.m_hasConfigurationData;

        /// <summary>
        /// Gets the configured weight
        /// </summary>
        [XmlAttribute("w"), JsonProperty("w")]
        public double ConfiguredWeight { get; set; }

        /// <summary>
        /// True if serialization of configured weight
        /// </summary>
        public bool ShouldSerializeConfiguredWeight() => this.m_hasConfigurationData;

        /// <summary>
        /// Gets the score assigned to this assertion
        /// </summary>
        [XmlAttribute("score"), JsonProperty("score")]
        public double Score { get; set; }

        /// <summary>
        /// Gets the score assigned to this assertion
        /// </summary>
        [XmlAttribute("a"), JsonProperty("a")]
        public String A { get; set; }

        /// <summary>
        /// Gets the score assigned to this assertion
        /// </summary>
        [XmlAttribute("b"), JsonProperty("b")]
        public String B { get; set; }

        /// <summary>
        /// Get the time this was modified
        /// </summary>
        public override DateTimeOffset ModifiedOn => DateTimeOffset.Now;
    }
}