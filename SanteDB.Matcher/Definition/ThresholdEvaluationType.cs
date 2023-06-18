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
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Threshold evaluation type
    /// </summary>
    [XmlType(nameof(ThresholdEvaluationType), Namespace = "http://santedb.org/matcher")]
    public enum ThresholdEvaluationType
    {
        /// <summary>
        /// The classification thresholds are calculated using the weighted scores between -min and +max
        /// </summary>
        [XmlEnum("score")]
        AbsoluteScore = 0,
        /// <summary>
        /// The classification thresholds are calculated using the value between 0..1
        /// </summary>
        [XmlEnum("strength")]
        Percentage = 1
    }
}