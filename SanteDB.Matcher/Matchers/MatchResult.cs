/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Serialization;
using SanteDB.Core.Model;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents a single record match
    /// </summary>
    /// <typeparam name="T">The type of record matched</typeparam>
    public class MatchResult<T> : IRecordMatchResult<T>
        where T: IdentifiedData
    {
        /// <summary>
        /// Creates a new match result
        /// </summary>
        /// <param name="record">The record that was classified</param>
        /// <param name="score">The assigned score</param>
        /// <param name="classification">The classification</param>
        public MatchResult(T record, double score, RecordMatchClassification classification, RecordMatchMethod method)
        {
            this.Record = record;
            this.Score = score;
            this.Classification = classification;
            this.Vectors = new List<MatchVector>();
            this.Method = method;
        }

        /// <summary>
        /// Gets the score
        /// </summary>
        public double Score { get; private set; }

        /// <summary>
        /// Gets the confidence that this is a match (the number of assertions that were actually assessed)
        /// </summary>
        public double EvaluatedVectors
        {
            get
            {
                return (double)this.Vectors.Count(o=>o.Evaluated) / this.Vectors.Count();
            }
        }

        /// <summary>
        /// Gets the record
        /// </summary>
        public T Record { get; private set; }

        /// <summary>
        /// Gets the classification 
        /// </summary>
        public RecordMatchClassification Classification { get; private set; }

        /// <summary>
        /// Gets or sets the properties that matched and their score
        /// </summary>
        public IList<MatchVector> Vectors { get; internal set; }

        /// <summary>
        /// Gets the method of match
        /// </summary>
        public RecordMatchMethod Method { get; private set; }

        /// <summary>
        /// Gets the record that matched
        /// </summary>
        IdentifiedData IRecordMatchResult.Record => this.Record;

        /// <summary>
        /// Represent this match as a string
        /// </summary>
        public override string ToString()
        {
            return $"{this.Classification} - {this.Record} (SCORE: {this.Score}, EVAL: {this.EvaluatedVectors}";
        }
    }

    /// <summary>
    /// Represents an individual property that matched
    /// </summary>
    [XmlType(Namespace = "http://santedb.org/matcher"), JsonObject]
    public class MatchVector
    {

        /// <summary>
        /// Creates a new assertion result
        /// </summary>
        public MatchVector(MatchAttribute attribute, String name, double configuredProbability, double configuredWeight, double score, bool evaluated, object aValue, object bValue)
        {
            this.Attribute = attribute;
            this.Name = name;
            this.ConfiguredProbability = configuredProbability;
            this.ConfiguredWeight = configuredWeight;
            this.Score = score;
            this.Evaluated = evaluated;
            this.A = aValue;
            this.B = bValue;
        }

        /// <summary>
        /// Gets the value of A
        /// </summary>
        public Object A { get; private set; }

        /// <summary>
        /// Gets the value of B
        /// </summary>
        public Object B { get; private set; }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MatchAttribute Attribute { get; set; }

        /// <summary>
        /// True if the assertion was evaluated
        /// </summary>
        public bool Evaluated { get; set; }

        /// <summary>
        /// Gets the configured probability
        /// </summary>
        public double ConfiguredProbability { get; private set; }

        /// <summary>
        /// Gets the configured weight
        /// </summary>
        public double ConfiguredWeight { get; private set; }

        /// <summary>
        /// Gets the score assigned to this assertion
        /// </summary>
        public double Score { get; private set; }

    }
}
