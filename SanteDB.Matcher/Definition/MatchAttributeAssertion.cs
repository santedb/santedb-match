/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Match attribute rule
    /// </summary>
    [XmlType(nameof(MatchAttributeAssertion), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchAttributeAssertion))]
    public class MatchAttributeAssertion
    {

        /// <summary>
        /// Operator for classifier
        /// </summary>
        [XmlAttribute("op"), JsonProperty("op")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the value of the comparator
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public double Value { get; set; }

        /// <summary>
        /// Value specified
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public bool ValueSpecified { get; set; }

        /// <summary>
        /// Gets or sets the transformations
        /// </summary>
        [XmlElement("transform"), JsonProperty("transform")]
        public List<MatchTransform> Transforms { get; set; }

        /// <summary>
        /// Rules in the attribute rule
        /// </summary>
        [XmlElement("assert"), JsonProperty("assert")]
        public List<MatchAttributeAssertion> Assertions { get; set; }

        /// <summary>
        /// Represent the assertion
        /// </summary>
        public override string ToString() => $"ASSERT: {this.Operator} {this.Value} [XFRM: {String.Join(",", this.Transforms.Select(o => o.ToString()))}";
    }
}