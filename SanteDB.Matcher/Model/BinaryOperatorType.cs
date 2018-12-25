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
    /// Gets or sets the binary expression operator
    /// </summary>
    [XmlType(nameof(BinaryOperatorType), Namespace = "http://santedb.org/matcher")]
    public enum BinaryOperatorType
    {
        [XmlEnum("eq")]
        Equal,
        [XmlEnum("lt")]
        LessThan,
        [XmlEnum("lte")]
        LessThanOrEqual,
        [XmlEnum("gt")]
        GreaterThan,
        [XmlEnum("gte")]
        GreaterThanOrEqual,
        [XmlEnum("ne")]
        NotEqual,
        [XmlEnum("and")]
        AndAlso,
        [XmlEnum("or")]
        OrElse,
        [XmlEnum("add")]
        Add,
        [XmlEnum("sub")]
        Subtract,
        [XmlEnum("is")]
        TypeIs
    }
}
