/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.Matcher.Definition;
using SanteDB.Matcher.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a match report
    /// </summary>
    [JsonObject, XmlType(nameof(MatchDiagnostics), Namespace = "http://santedb.org/matcher")]
    public class MatchDiagnostics
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        public MatchDiagnostics()
        {
        }

        /// <summary>
        /// Get the match diagnostics
        /// </summary>
        internal MatchDiagnostics(MatchSessionDiagnosticInfo diagnosticSession)
        {
            this.StartTime = diagnosticSession.StartOfSession.DateTime;
            this.EndTime = diagnosticSession.EndOfSession.Value.DateTime;
            this.Configuration = diagnosticSession.Configuration;
            this.Stages = new List<MatchDiagnosticsStageData>(diagnosticSession.Stages.Select(o => new MatchDiagnosticsStageData(o)));
        }

        /// <summary>
        /// Gets the start time of the session
        /// </summary>
        [XmlElement("start"), JsonProperty("start")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets the end time of the session
        /// </summary>
        [XmlElement("end"), JsonProperty("end")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets the configuration used to diagnose the match
        /// </summary>
        [XmlElement("configuration"), JsonProperty("configuration")]
        public MatchConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets the stages executed
        /// </summary>
        [XmlElement("stage"), JsonProperty("stages")]
        public List<MatchDiagnosticsStageData> Stages { get; set; }
    }

    /// <summary>
    /// Match diagnostics stage data
    /// </summary>
    [JsonObject, XmlType(nameof(MatchDiagnosticsStageData), Namespace = "http://santedb.org/matcher")]
    public class MatchDiagnosticsStageData
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        public MatchDiagnosticsStageData()
        {
        }

        /// <summary>
        /// Get the match diagnostics
        /// </summary>
        internal MatchDiagnosticsStageData(MatchStageDiagnosticInfo diagnosticsInfo)
        {
            this.StartTime = diagnosticsInfo.StartOfStage.DateTime;
            this.EndTime = diagnosticsInfo.EndOfStage.Value.DateTime;
            this.StageName = diagnosticsInfo.StageName;
            this.Actions = new List<MatchDiagnosticsActionData>(diagnosticsInfo.Actions.Select(o => new MatchDiagnosticsActionData(o)));
        }

        /// <summary>
        /// Gets the time that the stage started
        /// </summary>
        [XmlElement("start"), JsonProperty("start")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets the time the stage ended
        /// </summary>
        [XmlElement("end"), JsonProperty("end")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets the name of the stage
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String StageName { get; set; }

        /// <summary>
        /// Gets the actions
        /// </summary>
        [XmlElement("action"), JsonProperty("actions")]
        public List<MatchDiagnosticsActionData> Actions { get; set; }
    }

    /// <summary>
    /// Represents a single action that was taken
    /// </summary>
    [JsonObject, XmlType(nameof(MatchDiagnosticsActionData), Namespace = "http://santedb.org/matcher")]
    public class MatchDiagnosticsActionData
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        public MatchDiagnosticsActionData()
        {
        }

        /// <summary>
        /// Get the match diagnostics
        /// </summary>
        internal MatchDiagnosticsActionData(MatchActionDiagnosticInfo diagnosticsInfo)
        {
            this.StartTime = diagnosticsInfo.StartOfAction.DateTime;
            this.EndTime = diagnosticsInfo.EndOfAction.Value.DateTime;
            this.ActionType = diagnosticsInfo.ActionType;
            this.ActionIdentification = diagnosticsInfo.ActionData;
            this.Data = new List<MatchDiagnosticsSampleData>(diagnosticsInfo.Data.Select(o => new MatchDiagnosticsSampleData(o)));
            this.Actions = new List<MatchDiagnosticsActionData>(diagnosticsInfo.Actions.Select(o => new MatchDiagnosticsActionData(o)));
        }

        /// <summary>
        /// Gets the time that the stage started
        /// </summary>
        [XmlElement("start"), JsonProperty("start")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets the time the stage ended
        /// </summary>
        [XmlElement("end"), JsonProperty("end")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets the name of the stage
        /// </summary>
        [XmlElement("type"), JsonProperty("type")]
        public String ActionType { get; set; }

        /// <summary>
        /// Gets the name of the stage
        /// </summary>
        [XmlElement("id"), JsonProperty("id")]
        public String[] ActionIdentification { get; set; }

        /// <summary>
        /// Gets the actions
        /// </summary>
        [XmlElement("data"), JsonProperty("data")]
        public List<MatchDiagnosticsSampleData> Data { get; set; }

        /// <summary>
        /// Gets the child actions
        /// </summary>
        [XmlElement("child"), JsonProperty("children")]
        public List<MatchDiagnosticsActionData> Actions { get; set; }
    }

    /// <summary>
    /// Information about the data samples take
    /// </summary>
    [JsonObject, XmlType(nameof(MatchDiagnosticsSampleData), Namespace = "http://santedb.org/matcher")]
    public class MatchDiagnosticsSampleData : MatchDiagnosticsActionData
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        public MatchDiagnosticsSampleData()
        {
        }

        /// <summary>
        /// Create new diagnostics serialization data
        /// </summary>
        internal MatchDiagnosticsSampleData(MatchSampleDiagnosticInfo sampleData)
        {
            this.Timestamp = sampleData.Timestamp.DateTime;
            this.Key = sampleData.Key;
            this.Value = sampleData.Value;
        }

        /// <summary>
        /// The time the sample was taken
        /// </summary>
        [XmlElement("timestamp"), JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets the key of the sample
        /// </summary>
        [XmlElement("key"), JsonProperty("key")]
        public String Key { get; set; }

        /// <summary>
        /// Gets the value
        /// </summary>
        [JsonProperty("value"),
            XmlElement("string", typeof(String)),
            XmlElement("int", typeof(int)),
            XmlElement("float", typeof(float)),
            XmlElement("bool", typeof(bool)),
            XmlElement("double", typeof(double)),
            XmlElement("date", typeof(DateTime))
            ]
        public object Value { get; set; }
    }
}