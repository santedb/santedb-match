/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Gets or sets the types of binary operators that can be applied in a matching configuration
    /// </summary>
    /// <remarks>A binary statement as defined in <see cref="MatchBlock"/> or <see cref="MatchAttributeAssertion"/> dictates how the $input
    /// and $block values are evaluated.</remarks>
    [XmlType(nameof(BinaryOperatorType), Namespace = "http://santedb.org/matcher")]
    public enum BinaryOperatorType
    {
        /// <summary>
        /// $input and $block must equal
        /// </summary>
        [XmlEnum("eq")]
        Equal,
        /// <summary>
        /// $input must be less than $block
        /// </summary>
        [XmlEnum("lt")]
        LessThan,
        /// <summary>
        /// $input must be less than or equal to $block
        /// </summary>
        [XmlEnum("lte")]
        LessThanOrEqual,
        /// <summary>
        /// $input must be greater than $block
        /// </summary>
        [XmlEnum("gt")]
        GreaterThan,
        /// <summary>
        /// $input must be greater than or equal to $block
        /// </summary>
        [XmlEnum("gte")]
        GreaterThanOrEqual,
        /// <summary>
        /// $input must not be equal to $block
        /// </summary>
        [XmlEnum("ne")]
        NotEqual,
        /// <summary>
        /// Used when an assertion contains multiple statements. Each statement in the block must be true
        /// </summary>
        [XmlEnum("and")]
        AndAlso,
        /// <summary>
        /// Used when an assertion contains multiple statements. One of the statements in the block must true
        /// </summary>
        [XmlEnum("or")]
        OrElse
    }
}
