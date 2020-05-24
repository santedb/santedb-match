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
using SanteDB.Matcher.Matchers;
using System;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a match vector
    /// </summary>
    [XmlType(nameof(VectorResultReport), Namespace = "http://santedb.org/matcher"), JsonObject]
    public class VectorResultReport
    {

        // Vector result
        private VectorResult m_result;

        /// <summary>
        /// Default ctor for serializer
        /// </summary>
        public VectorResultReport()
        {
        }

        /// <summary>
        /// Creates a new result report from the specified result
        /// </summary>
        public VectorResultReport(VectorResult result)
        {
            this.m_result = result;
        }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name
        {
            get => this.m_result.Name;
            set { }
        }

        /// <summary>
        /// True if the vector was evaluated
        /// </summary>
        [XmlAttribute("evaluated"), JsonProperty("evaluated")]
        public bool Evaluated
        {
            get => this.m_result.Evaluated;
            set { }
        }

        /// <summary>
        /// Gets the configured probability
        /// </summary>
        [XmlAttribute("m"), JsonProperty("m")]
        public double ConfiguredProbability
        {
            get => this.m_result.ConfiguredProbability;
            set { }
        }


        /// <summary>
        /// Gets the configured weight
        /// </summary>
        [XmlAttribute("w"), JsonProperty("w")]
        public double ConfiguredWeight
        {
            get => this.m_result.ConfiguredWeight;
            set { }
        }

        /// <summary>
        /// Gets the score assigned to this vector
        /// </summary>
        [XmlAttribute("score"), JsonProperty("score")]
        public double Score
        {
            get => this.m_result.Score;
            set { }
        }

        /// <summary>
        /// Gets the score assigned to this vector
        /// </summary>
        [XmlAttribute("a"), JsonProperty("a")]
        public String A
        {
            get => this.m_result.A?.ToString();
            set { }
        }

        /// <summary>
        /// Gets the score assigned to this vector
        /// </summary>
        [XmlAttribute("b"), JsonProperty("b")]
        public String B
        {
            get => this.m_result.B.ToString();
            set { }
        }


    }
}