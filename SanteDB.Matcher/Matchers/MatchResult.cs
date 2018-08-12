﻿using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents a single record match
    /// </summary>
    /// <typeparam name="T">The type of record matched</typeparam>
    public class MatchResult<T> : IRecordMatchResult<T>
    {
        /// <summary>
        /// Creates a new match result
        /// </summary>
        /// <param name="record">The record that was classified</param>
        /// <param name="score">The assigned score</param>
        /// <param name="classification">The classification</param>
        public MatchResult(T record, double score, RecordMatchClassification classification)
        {
            this.Record = record;
            this.Score = score;
            this.Classification = classification;
            this.Vectors = new List<VectorResult>();
        }

        /// <summary>
        /// Gets the score
        /// </summary>
        public double Score { get; private set; }

        /// <summary>
        /// Gets the confidence that this is a match (the number of vectors that were actually assessed)
        /// </summary>
        public double Confidence
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
        public List<VectorResult> Vectors { get; private set; }

        public override string ToString()
        {
            return $"{this.Classification} - {this.Record} (SCORE: {this.Score}, CONF: {this.Confidence}";
        }
    }

    /// <summary>
    /// Represents an individual property that matched
    /// </summary>
    public class VectorResult
    {

        /// <summary>
        /// Creates a new vector result
        /// </summary>
        public VectorResult(String name, double configuredProbability, double configuredWeight, double score, bool evaluated)
        {
            this.Name = name;
            this.ConfiguredProbability = configuredProbability;
            this.ConfiguredWeight = configuredWeight;
            this.Score = score;
            this.Evaluated = evaluated;
        }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// True if the vector was evaluated
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
        /// Gets the score assigned to this vector
        /// </summary>
        public double Score { get; private set; }

    }
}
