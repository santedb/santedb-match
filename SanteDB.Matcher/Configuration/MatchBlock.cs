﻿/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
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
 * User: JustinFyfe
 * Date: 2019-1-22
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Represents a block configuration which are IMSI expressions to send to the database
    /// </summary>
    [XmlType(nameof(MatchBlock), Namespace = "http://santedb.org/matcher")]
    public class MatchBlock
    {

        /// <summary>
        /// Gets or sets the binary operator
        /// </summary>
        [XmlAttribute("op")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the block filters
        /// </summary>
        [XmlElement("imsiExpression")]
        public List<String> Filter { get; set; }

        /// <summary>
        /// Gets or sets the maximum results for this filter
        /// </summary>
        [XmlAttribute("maxResults")]
        public int MaxReuslts { get; set; }
    }
}