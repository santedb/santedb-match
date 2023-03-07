/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2022-5-30
 */
using Newtonsoft.Json;
using SanteDB.Core.Matching;
using SanteDB.Core.Model;
using SanteDB.Matcher.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Matchers
{

    /// <summary>
    /// Non-generic version of match result
    /// </summary>
    public abstract class MatchResult : IRecordMatchResult
    {

        /// <summary>
        /// Creates a new match result
        /// </summary>
        /// <param name="record">The record that was classified</param>
        /// <param name="score">The assigned score</param>
        /// <param name="classification">The classification</param>
        /// <param name="method">The method that was used to establish the match</param>
        /// <param name="strength">The relative strength (0 .. 1) of the match given the maximum score the match could have gotten</param>
        /// <param name="vectors">The attributes + scores</param>
        /// <param name="configuration">The name of the configuration used to match</param>
        public MatchResult(IdentifiedData record, double score, double strength, IRecordMatchingConfiguration configuration, RecordMatchClassification classification, RecordMatchMethod method, IEnumerable<IRecordMatchVector> vectors)
        {
            this.Strength = strength;
            this.Record = record;
            this.Score = score;
            this.Classification = classification;
            this.Configuration = configuration;
            this.Vectors = vectors.Select(o => o is MatchVector mv ? mv : new MatchVector(o)).ToList();
            this.Method = method;
        }

        /// <summary>
        /// Gets the configuration name used to match
        /// </summary>
        public IRecordMatchingConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the record that was matched
        /// </summary>
        public IdentifiedData Record { get; private set; }

        /// <summary>
        /// Gets the numeric score.
        /// </summary>
        public double Score { get; private set; }

        /// <summary>
        /// Gets the strength of the vector match (0..1)
        /// </summary>
        public double Strength { get; private set; }

        /// <summary>
        /// Gets the classification
        /// </summary>
        public RecordMatchClassification Classification { get; private set; }

        /// <summary>
        /// Gets the method that was used to calculate the vector's scopre
        /// </summary>
        public RecordMatchMethod Method { get; private set; }

        /// <summary>
        /// Get the generic vectors
        /// </summary>
        IEnumerable<IRecordMatchVector> IRecordMatchResult.Vectors => this.Vectors;

        /// <summary>
        /// Gets or sets the properties that matched and their score
        /// </summary>
        public IList<MatchVector> Vectors { get; private set; }


        /// <summary>
        /// Represent this match as a string
        /// </summary>
        public override string ToString()
        {
            return $"{this.Classification} - {this.Record} (SCORE: {this.Score}, STR: {this.Strength}";
        }
    }

    /// <summary>
    /// Represents a single record match
    /// </summary>
    /// <typeparam name="T">The type of record matched</typeparam>
    public class MatchResult<T> : MatchResult, IRecordMatchResult<T>
        where T : IdentifiedData
    {
        /// <summary>
        /// Creates a new match result
        /// </summary>
        /// <param name="record">The record that was classified</param>
        /// <param name="score">The assigned score</param>
        /// <param name="classification">The classification</param>
        /// <param name="method">The method that was used to establish the match</param>
        /// <param name="strength">The relative strength (0 .. 1) of the match given the maximum score the match could have gotten</param>
        /// <param name="configuration">The name of the configuration used to score</param>
        /// <param name="vectors">The matching attribute and scores</param>
        public MatchResult(T record, double score, double strength, IRecordMatchingConfiguration configuration, RecordMatchClassification classification, RecordMatchMethod method, IEnumerable<IRecordMatchVector> vectors)
            : base(record, score, strength, configuration, classification, method, vectors)
        {
        }

        /// <summary>
        /// Gets the record
        /// </summary>
        public new T Record => (T)base.Record;

    }

    /// <summary>
    /// Represents an individual property that matched
    /// </summary>
    [XmlType(Namespace = "http://santedb.org/matcher"), JsonObject]
    public class MatchVector : IRecordMatchVector
    {

        /// <summary>
        /// Create new match vector form a generic vector interface
        /// </summary>
        public MatchVector(IRecordMatchVector vector)
        {
            this.Name = vector.Name;
            this.Score = vector.Score;
            this.A = vector.A;
            this.B = vector.B;
            this.Evaluated = vector.Evaluated;
        }

        /// <summary>
        /// Creates a new assertion result
        /// </summary>
        public MatchVector(MatchAttribute attribute, String name, double score, bool evaluated, object aValue, object bValue)
        {
            this.Attribute = attribute;
            this.Name = name;
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
        /// Gets the score assigned to this assertion
        /// </summary>
        public double Score { get; internal set; }
        
    }
}
