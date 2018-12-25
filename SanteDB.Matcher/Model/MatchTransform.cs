﻿/*
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
    /// Match transform configuration
    /// </summary>
    [XmlType(nameof(MatchTransform), Namespace = "http://santedb.org/matcher")]
    public class MatchTransform
    {

        /// <summary>
        /// The registered name of the transformer
        /// </summary>
        [XmlAttribute("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the match transformation parameter
        /// </summary>
        [XmlArray("args")]
        [XmlArrayItem("int", typeof(int))]
        [XmlArrayItem("double", typeof(double))]
        [XmlArrayItem("string", typeof(string))]
        [XmlArrayItem("boolean", typeof(bool))]
        public List<Object> Parameters { get; set; }
    }
}