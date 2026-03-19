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
using SanteDB.Core.BusinessRules;
using SanteDB.Core.Configuration;
using SanteDB.Matcher.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Configuration
{
    /// <summary>
    /// Configures the file matching configuration
    /// </summary>
    [XmlType(nameof(FileMatchConfigurationSection), Namespace = "http://santedb.org/configuration")]
    [JsonObject(nameof(FileMatchConfigurationSection))]
    public class FileMatchConfigurationSection : IConfigurationSection, IValidatableConfigurationSection
    {

        /// <summary>
        /// True if files should be cached
        /// </summary>
        [XmlAttribute("cache"), JsonProperty("cache")]
        public bool CacheFiles { get; set; }

        /// <summary>
        /// File path
        /// </summary>
        [XmlArray("basePath"), XmlArrayItem("add"), JsonProperty("basePath")]
        public List<FilePathConfiguration> FilePath { get; set; }


        /// <inheritdoc/>
        public IEnumerable<DetectedIssue> Validate()
        {
            foreach (var path in this.FilePath.Select(o => o.Path.ToLower()))
            {
                if (!Directory.Exists(path)) // HACK: Might be on linux or have a lower case data file
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Warning, "err.config.folder", $"Folder {path} doesn't exist", Guid.Empty);
                }
                else if (!Directory.EnumerateFiles(path, "*.xml").Any())
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Warning, "err.config.nods", $"Folder path {path} does not contain any match configurations", Guid.Empty);
                }
                
                if (!Path.IsPathRooted(path))
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Warning, "err.config.abs", $"Folder path {path} should be absolute", Guid.Empty);
                }
                
            }
        }
    }
}
