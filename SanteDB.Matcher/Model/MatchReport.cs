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
 * Date: 2023-5-19
 */
using Newtonsoft.Json;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Acts;
using SanteDB.Core.Model.Attributes;
using SanteDB.Core.Model.Entities;
using SanteDB.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a match report
    /// </summary>
    [JsonObject, XmlType(nameof(MatchReport), Namespace = "http://santedb.org/matcher"), XmlRoot(nameof(MatchReport), Namespace = "http://santedb.org/matcher")]
    [XmlInclude(typeof(Act))]
    [XmlInclude(typeof(TextObservation))]
    [XmlInclude(typeof(CodedObservation))]
    [XmlInclude(typeof(QuantityObservation))]
    [XmlInclude(typeof(PatientEncounter))]
    [XmlInclude(typeof(SubstanceAdministration))]
    [XmlInclude(typeof(Entity))]
    [XmlInclude(typeof(Patient))]
    [XmlInclude(typeof(Person))]
    [XmlInclude(typeof(Provider))]
    [XmlInclude(typeof(Organization))]
    [XmlInclude(typeof(Place))]
    [XmlInclude(typeof(Material))]
    [XmlInclude(typeof(ManufacturedMaterial))]
    [XmlInclude(typeof(DeviceEntity))]
    [XmlInclude(typeof(UserEntity))]
    [XmlInclude(typeof(ApplicationEntity))]
    [AddDependentSerializersAttribute]
    public class MatchReport : IdentifiedData
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        public MatchReport()
        {
        }

        /// <summary>
        /// The input
        /// </summary>
        [XmlElement("input"), JsonProperty("input")]
        public Guid Input { get; set; }

        /// <summary>
        /// The results for the matches
        /// </summary>
        [XmlElement("result"), JsonProperty("results")]
        public List<MatchResultReport> Results { get; set; }

        /// <summary>
        /// Gets the diagnostics for this match report
        /// </summary>
        [XmlElement("diagnostics"), JsonProperty("diagnostics")]
        public MatchDiagnostics Diagnostics { get; set; }

        /// <summary>
        /// Gets the date of modification
        /// </summary>
        public override DateTimeOffset ModifiedOn => DateTimeOffset.Now;
    }
}