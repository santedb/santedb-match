/*
 *
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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{

    /// <summary>
    /// Match measure source
    /// </summary>
    [XmlType(nameof(MatchMeasureSource), Namespace = "http://santedb.org/matcher")]
    public enum MatchMeasureSource
    {
        [XmlEnum("asserted")]
        Assertion,
        [XmlEnum("attribute")]
        Attribute
    }

    /// <summary>
    /// Match measurement transform
    /// </summary>
    [XmlType(nameof(MatchMeasureTransform), Namespace = "http://santedb.org/matcher")]
    public class MatchMeasureTransform : MatchTransform
    {

        /// <summary>
        /// The source value
        /// </summary>
        [XmlElement("source"), JsonProperty("source")]
        public MatchMeasureSource SourceValue { get; set; }

        /// <summary>
        /// Gets or sets the transformations
        /// </summary>
        [XmlElement("transform"), JsonProperty("transform")]
        public List<MatchTransform> Transforms { get; set; }

    }
}