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
using SanteDB.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Represents a target of a match (if applicable)
    /// </summary>
    [XmlType(nameof(MatchTarget), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchTarget))]
    public class MatchTarget
    {

        // The type 
        private Type m_type = null;

        /// <summary>
        /// Gets or sets the resource xml
        /// </summary>
        [XmlAttribute("resource"), JsonProperty("resource")]
        public string ResourceXml { get; set; }

        /// <summary>
        /// The resource type
        /// </summary>
        [XmlIgnore]
        public Type ResourceType
        {
            get
            {
                if (this.m_type == null)
                {
                    this.m_type = typeof(Patient).Assembly.GetExportedTypesSafe().FirstOrDefault(o => o.GetCustomAttribute<XmlRootAttribute>()?.ElementName == this.ResourceXml);
                }

                return this.m_type;
            }
        }

        /// <summary>
        /// The event on which the auto-matching should be triggered
        /// </summary>
        [XmlElement("event"), JsonProperty("event")]
        public List<String> Event { get; set; }


    }
}