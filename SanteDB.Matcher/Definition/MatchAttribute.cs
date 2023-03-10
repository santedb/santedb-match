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
using SanteDB.Core.Model.Query;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Definition
{
    /// <summary>
    /// Represents a configured match attribute
    /// </summary>
    [XmlType(nameof(MatchAttribute), Namespace = "http://santedb.org/matcher")]
    [JsonObject(nameof(MatchAttribute))]
    public class MatchAttribute
    {
        private double? m_m = null;
        private double? m_u = null;
        private double? m_weightN = null;
        private double? m_weightM = null;

        private ConcurrentDictionary<Type, IDictionary> m_propertySelectors = new ConcurrentDictionary<Type, IDictionary>();

        /// <summary>
        /// The identifier for the attribute
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("description"), JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// When condition on a match attribute
        /// </summary>
        [XmlArray("when"), XmlArrayItem("attribute"), JsonProperty("when")]
        public List<MatchAttributeWhenCondition> When { get; set; }

        /// <summary>
        /// Gets or sets the match weight
        /// </summary>
        [XmlAttribute("m"), JsonProperty("m")]
        public double M
        {
            get => this.m_m.GetValueOrDefault();
            set => this.m_m = value;
        }

        /// <summary>
        /// Gets or sets the unmatch weight (penalty)
        /// </summary>
        [XmlAttribute("u"), JsonProperty("u")]
        public double U
        {
            get => this.m_u.GetValueOrDefault();
            set => this.m_u = value;
        }

        /// <summary>
        /// Gets or sets the match weight
        /// </summary>
        [XmlAttribute("matchWeight"), JsonProperty("matchWeight")]
        public double MatchWeight
        {
            get
            {
                if (!this.m_weightM.HasValue)
                {
                    // U must be set
                    if (!this.m_u.HasValue || !this.m_m.HasValue)
                    {
                        throw new InvalidOperationException("U and M variables must be set");
                    }

                    this.m_weightM = (this.m_m.Value / this.m_u.Value).Ln() / (2.0d).Ln();
                }
                return this.m_weightM.Value;
            }
        }

        /// <summary>
        /// Gets or sets the unmatch weight (penalty)
        /// </summary>
        [XmlAttribute("nonMatchWeight"), JsonProperty("nonMatchWeight")]
        public double NonMatchWeight
        {
            get
            {
                if (!this.m_weightN.HasValue)
                {
                    // U must be set
                    if (!this.m_u.HasValue || !this.m_m.HasValue)
                    {
                        throw new InvalidOperationException("U and M variables must be set");
                    }

                    this.m_weightN = ((1 - this.m_m.Value) / (1 - this.m_u.Value)).Ln() / (2.0d).Ln();
                }
                return this.m_weightN.Value;
            }
        }

        /// <summary>
        /// Gets or sets the property
        /// </summary>
        [XmlAttribute("property"), JsonProperty("property")]
        public List<string> Property { get; set; }

        /// <summary>
        /// Property selector helper
        /// </summary>
        public IDictionary<String, Func<T, dynamic>> GetPropertySelectors<T>()
        {
            if (!this.m_propertySelectors.TryGetValue(typeof(T), out IDictionary retVal))
            {
                retVal = this.Property.ToDictionary(o => o, o =>
                {
                    var selector = QueryExpressionParser.BuildPropertySelector<T>(o, true);
                    var param = Expression.Parameter(typeof(T));
                    return Expression.Lambda<Func<T, dynamic>>(Expression.Convert(Expression.Invoke(selector, param), typeof(Object)), param).Compile();
                });
                this.m_propertySelectors.TryAdd(typeof(T), retVal);
            }
            return retVal as IDictionary<String, Func<T, dynamic>>;
        }

        /// <summary>
        /// When null what should happen?
        /// </summary>
        [XmlAttribute("whenNull"), JsonProperty("whenNull")]
        public MatchAttributeNullBehavior WhenNull { get; set; }

        /// <summary>
        /// The match has to be executed, if the inbound object does not have the property (is null)
        /// then the entire matching process stops.
        /// </summary>
        [XmlAttribute("required"), JsonProperty("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        [XmlElement("assert"), JsonProperty("assert")]
        public MatchAttributeAssertion Assertion { get; set; }

        /// <summary>
        /// Gets or sets the algorithm for partially measuring the value
        /// </summary>
        [XmlElement("partialWeight"), JsonProperty("partialWeight")]
        public MatchMeasureTransform Measure { get; set; }

        /// <summary>
        /// Represent this attribute as a string
        /// </summary>
        public override string ToString() => $"ATTRIBUTE: {String.Join(" || ", this.Property)} ({this.Assertion}) [match: {this.MatchWeight}, non: {this.NonMatchWeight}, u: {this.U}, m: {this.M}, measure: {this.Measure}]";
    }
}