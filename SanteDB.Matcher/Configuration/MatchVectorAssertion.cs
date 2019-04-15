/*
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
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Match vector rule
    /// </summary>
    [XmlType(nameof(MatchVectorAssertion), Namespace = "http://santedb.org/matcher")]
    public class MatchVectorAssertion
    {

        /// <summary>
        /// Operator for classifier
        /// </summary>
        [XmlAttribute("op")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the value of the comparator
        /// </summary>
        [XmlAttribute("value")]
        public double Value { get; set; }

        /// <summary>
        /// Value specified
        /// </summary>
        [XmlIgnore]
        public bool ValueSpecified { get; set; }

        /// <summary>
        /// Gets or sets the transformations
        /// </summary>
        [XmlElement("transform")]
        public List<MatchTransform> Transforms { get; set; }

        /// <summary>
        /// Rules in the vector rule
        /// </summary>
        [XmlElement("assert")]
        public List<MatchVectorAssertion> Assertions { get; set; }

    }
}