/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Represents a block configuration which are IMSI expressions to send to the database
    /// </summary>
    [XmlType(nameof(MatchBlock), Namespace = "http://santedb.org/matcher"), JsonObject(nameof(MatchBlock))]
    public class MatchBlock
    {
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the binary operator
        /// </summary>
        [XmlAttribute("op"), JsonProperty("op")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the block filters
        /// </summary>
        [XmlElement("filter"), JsonProperty("filter")]
        public List<MatchBlockFilterExpression> Filter { get; set; }

        /// <summary>
        /// Gets or sets the maximum results for this filter
        /// </summary>
        [XmlAttribute("maxResults"), JsonProperty("maxResults")]
        public int BatchSize { get; set; }

        /// <summary>
        /// When true, skip this blocking filter if all the values are null
        /// </summary>
        [XmlAttribute("skipWhenNullInput"), JsonProperty("skipWhenNullInput")]
        public bool SkipIfNull { get; set; }
    }
}
