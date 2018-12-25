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
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents behaviors when a vector property is null
    /// </summary>
    [XmlType(nameof(MatchVectorNullBehavior), Namespace = "http://santedb.org/matcher")]
    public enum MatchVectorNullBehavior
    {
        /// <summary>
        /// When the field is null on the queried record apply the mWeight
        /// </summary>
        [XmlEnum("match")]
        Match,
        /// <summary>
        /// When the property is null on the queried record apply the uWeight
        /// </summary>
        [XmlEnum("nonmatch")]
        NonMatch,
        /// <summary>
        /// When the property is null on the queried record ignore the rule
        /// </summary>
        [XmlEnum("ignore")]
        Ignore,
        /// <summary>
        /// When the property is null on the queried record, disqualify the match entirely
        /// </summary>
        [XmlEnum("disqualify")]
        Disqualify

    }
}
