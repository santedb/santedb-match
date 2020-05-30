﻿/*
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
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Vector when condition
    /// </summary>
    [XmlType(nameof(MatchVectorWhenCondition), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchVectorWhenCondition))]
    public class MatchVectorWhenCondition
    {

        /// <summary>
        /// The referenced vector
        /// </summary>
        [XmlAttribute("ref"), JsonProperty("ref")]
        public string VectorRef { get; set; }

        /// <summary>
        /// Operator for classifier
        /// </summary>
        [XmlAttribute("op"), JsonProperty("op")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the value of the comparator
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public bool Value { get; set; }
    }
}