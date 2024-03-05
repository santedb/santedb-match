/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.Core;
using SanteDB.Core.Matching;
using SanteDB.Core.Model;
using SanteDB.Matcher.Definition;
using SanteDB.Matcher.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanteDB.Matcher.Diagnostics
{
    /// <summary>
    /// Overall match session information
    /// </summary>
    internal sealed class MatchSessionDiagnosticInfo
    {
        /// <summary>
        /// Match session information
        /// </summary>
        public MatchSessionDiagnosticInfo(MatchConfiguration configuration)
        {
            this.Configuration = configuration;
            this.StartOfSession = DateTimeOffset.Now;
            this.EndOfSession = null;
            this.Stages = new List<MatchStageDiagnosticInfo>();
        }

        /// <summary>
        /// Match session information
        /// </summary>
        public MatchSessionDiagnosticInfo(String configurationName)
        {
            this.Configuration = new MatchConfiguration() { Id = configurationName };
            this.StartOfSession = DateTimeOffset.Now;
            this.EndOfSession = null;
            this.Stages = new List<MatchStageDiagnosticInfo>();
        }

        /// <summary>
        /// End the session information
        /// </summary>
        public void End()
        {
            this.EndOfSession = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets the configuration
        /// </summary>
        public MatchConfiguration Configuration { get; }

        /// <summary>
        /// Gets the start of the diagnostic session
        /// </summary>
        public DateTimeOffset StartOfSession { get; }

        /// <summary>
        /// Gets the end of the diagnostics session
        /// </summary>
        public DateTimeOffset? EndOfSession { get; private set; }

        /// <summary>
        /// Get the stages
        /// </summary>
        public List<MatchStageDiagnosticInfo> Stages { get; }
    }

    /// <summary>
    /// Match stage session information
    /// </summary>
    internal sealed class MatchStageDiagnosticInfo
    {
        /// <summary>
        /// Match stage session info
        /// </summary>
        public MatchStageDiagnosticInfo(String name)
        {
            this.StageName = name;
            this.StartOfStage = DateTimeOffset.Now;
            this.EndOfStage = null;
            this.Actions = new List<MatchActionDiagnosticInfo>();
        }

        /// <summary>
        /// Sets the end of stage
        /// </summary>
        public void End()
        {
            this.EndOfStage = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets the name of the stage
        /// </summary>
        public String StageName { get; }

        /// <summary>
        /// Gets the start of the stage
        /// </summary>
        public DateTimeOffset StartOfStage { get; }

        /// <summary>
        /// Gets or sets the end of the stage
        /// </summary>
        public DateTimeOffset? EndOfStage { get; private set; }

        /// <summary>
        /// Gets the actions in the stage
        /// </summary>
        public List<MatchActionDiagnosticInfo> Actions { get; }
    }

    /// <summary>
    /// The action information
    /// </summary>
    internal sealed class MatchActionDiagnosticInfo
    {
        /// <summary>
        /// Creates a new match action diagnostic information structure
        /// </summary>
        public MatchActionDiagnosticInfo()
        {
            this.EndOfAction = null;
            this.Data = new List<MatchSampleDiagnosticInfo>();
            this.Actions = new List<MatchActionDiagnosticInfo>();
        }

        /// <summary>
        /// Create match action info from block
        /// </summary>
        /// <param name="block">The block to create the action information for</param>
        public MatchActionDiagnosticInfo(MatchBlock block) : this()
        {
            this.ActionType = "blocking-instruction";
            this.ActionData = block.Filter.Select(o => o.Expression).ToArray();
            this.StartOfAction = DateTimeOffset.Now;
        }

        /// <summary>
        /// Create match action info from block
        /// </summary>
        /// <param name="blockRecord">The block to create the action information for</param>
        public MatchActionDiagnosticInfo(IdentifiedData blockRecord) : this()
        {
            this.ActionType = "classify-block";
            this.ActionData = new string[] { blockRecord.ToString() };
            this.StartOfAction = DateTimeOffset.Now;
        }

        /// <summary>
        /// Create match action info from block
        /// </summary>
        /// <param name="attribute">The attribute</param>
        public MatchActionDiagnosticInfo(MatchAttribute attribute) : this()
        {
            this.ActionType = "evaluate-attribute";
            this.ActionData = attribute.Property.Union(new string[] { attribute.Id }).ToArray();
            this.StartOfAction = DateTimeOffset.Now;
        }

        /// <summary>
        /// Create match action info from block
        /// </summary>
        /// <param name="attributeId">The attribute</param>
        public MatchActionDiagnosticInfo(String attributeId) : this()
        {
            this.ActionType = attributeId;
            this.ActionData = null;
            this.StartOfAction = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets the action key
        /// </summary>
        public String ActionType { get; }

        /// <summary>
        /// Gets the action identifier
        /// </summary>
        public String[] ActionData { get; }

        /// <summary>
        /// Gets the start of the action
        /// </summary>
        public DateTimeOffset StartOfAction { get; }

        /// <summary>
        /// Gets the end of the action
        /// </summary>
        public DateTimeOffset? EndOfAction { get; private set; }

        /// <summary>
        /// Gets the data in the action
        /// </summary>
        public List<MatchSampleDiagnosticInfo> Data { get; private set; }

        /// <summary>
        /// Gets the data in the action
        /// </summary>
        public List<MatchActionDiagnosticInfo> Actions { get; private set; }

        /// <summary>
        /// End the action
        /// </summary>
        public void End()
        {
            this.EndOfAction = DateTimeOffset.Now;
        }
    }

    /// <summary>
    /// Match data diagnostic information
    /// </summary>
    internal sealed class MatchSampleDiagnosticInfo
    {
        /// <summary>
        /// Create new diagnostic information
        /// </summary>
        public MatchSampleDiagnosticInfo(String key, Object value)
        {
            this.Key = key;
            this.Value = value;
            this.Timestamp = DateTimeOffset.Now;
        }

        /// <summary>
        /// Get the key
        /// </summary>
        public String Key { get; }

        /// <summary>
        /// Get the value
        /// </summary>
        public Object Value { get; }

        /// <summary>
        /// Gets the time the data was logged
        /// </summary>
        public DateTimeOffset Timestamp { get; }
    }

    /// <summary>
    /// Match diagnostic session
    /// </summary>
    public class MatchDiagnosticSession : IRecordMatchingDiagnosticSession
    {
        // Session
        private MatchSessionDiagnosticInfo m_session;

        // Current action
        private Stack<MatchActionDiagnosticInfo> m_currentAction = new Stack<MatchActionDiagnosticInfo>();

        // Current stage
        private MatchStageDiagnosticInfo m_currentStage;

        /// <summary>
        /// Get the captured session data
        /// </summary>
        public object GetSessionData()
        {
            if (this.m_session == null)
            {
                throw new InvalidOperationException("No session data captured");
            }
            else if (!this.m_session.EndOfSession.HasValue)
            {
                throw new InvalidOperationException("Session has not finished");
            }

            return new MatchDiagnostics(this.m_session);
        }

        /// <inheritdoc/>
        public void LogEnd()
        {
            if (this.m_session == null)
            {
                throw new InvalidOperationException("No active session");
            }
            else if (this.m_session.EndOfSession.HasValue)
            {
                throw new InvalidOperationException("Session has already ended");
            }

            this.m_session.End();
        }

        /// <inheritdoc/>
        public void LogEndAction()
        {
            if (!this.m_currentAction.Any())
            {
                throw new InvalidOperationException("No active action");
            }

            this.m_currentAction.Pop().End();
        }

        /// <inheritdoc/>
        public void LogEndStage()
        {
            if (this.m_currentStage == null)
            {
                throw new InvalidOperationException("No current stage");
            }

            this.m_currentStage.End();
            this.m_currentStage = null;
        }

        /// <inheritdoc/>
        public void LogSample<T>(string key, T data)
        {
            if (this.m_currentAction == null)
            {
                throw new InvalidOperationException("No current action to log against");
            }
            this.m_currentAction.Peek().Data.Add(new MatchSampleDiagnosticInfo(key, data));
        }

        /// <inheritdoc/>
        public void LogStart(string configurationName)
        {
            if (this.m_session != null)
            {
                throw new InvalidOperationException("Session is already started on this logger");
            }

            var matchConfiguration = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>().GetConfiguration(configurationName) as MatchConfiguration;
            if (matchConfiguration != null)
            {
                this.m_session = new MatchSessionDiagnosticInfo(matchConfiguration);
            }
            else
            {
                // Some other configuration
                this.m_session = new MatchSessionDiagnosticInfo(configurationName);
            }
        }

        /// <inheritdoc/>
        public void LogStartAction(object counterTag)
        {
            MatchActionDiagnosticInfo current = null;
            if (this.m_currentStage == null)
            {
                throw new InvalidOperationException("No stage has been started");
            }
            else if (this.m_currentAction.Any())
            {
                current = this.m_currentAction.Peek();
            }

            switch (counterTag)
            {
                case IdentifiedData id:
                    this.m_currentAction.Push(new MatchActionDiagnosticInfo(id));
                    break;

                case MatchBlock mb:
                    this.m_currentAction.Push(new MatchActionDiagnosticInfo(mb));
                    break;

                case MatchAttribute ma:
                    this.m_currentAction.Push(new MatchActionDiagnosticInfo(ma));
                    break;

                case String st:
                    this.m_currentAction.Push(new MatchActionDiagnosticInfo(st));
                    break;

                default:
                    throw new InvalidOperationException($"Don't know how to log action {counterTag.GetType()}");
            }

            if (current != null)
            {
                current.Actions.Add(this.m_currentAction.Peek());
            }
            else
            {
                this.m_currentStage.Actions.Add(this.m_currentAction.Peek());
            }
        }

        /// <inheritdoc/>
        public void LogStartStage(string stageId)
        {
            if (this.m_session == null)
            {
                throw new InvalidOperationException("No session is started");
            }
            else if (this.m_currentStage != null)
            {
                throw new InvalidOperationException("Previous stage has not been ended via LogEndStage()");
            }
            this.m_currentStage = new MatchStageDiagnosticInfo(stageId);
            this.m_session.Stages.Add(this.m_currentStage);
        }
    }
}