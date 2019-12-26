using Newtonsoft.Json;
using SanteDB.Core.Model;
using SanteDB.Core.Services;
using SanteDB.Matcher.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a MatchResultReport which encapsulates a MatchResult
    /// </summary>
    [JsonObject, XmlType(nameof(MatchResultReport), Namespace = "http://santedb.org/matcher")]
    public class MatchResultReport
    {

        // The match data
        private MatchResult<IdentifiedData> m_match;

        /// <summary>
        /// Default ctor for serialization
        /// </summary>
        public MatchResultReport()
        {
        }

        /// <summary>
        /// Creates a new match result report from the specified match data
        /// </summary>
        public MatchResultReport(MatchResult<IdentifiedData> match)
        {
            this.m_match = match;
        }

        /// <summary>
        /// Gets the score
        /// </summary>
        [XmlElement("score"), JsonProperty("score")]
        public double Score
        {
            get => this.m_match.Score;
            set { }
        }

        /// <summary>
        /// Gets the confidence that this is a match (the number of vectors that were actually assessed)
        /// </summary>
        [XmlElement("evaluated"), JsonProperty("evaluated")]
        public double EvaluatedVectors
        {
            get => this.m_match.EvaluatedVectors;
            set { }
        }

        /// <summary>
        /// Gets the record
        /// </summary>
        [XmlElement("record"), JsonProperty("record")]
        public IdentifiedData Record
        {
            get => this.m_match.Record;
            set { }
        }

        /// <summary>
        /// Gets the classification 
        /// </summary>
        [XmlElement("classification"), JsonProperty("classification")]
        public RecordMatchClassification Classification
        {
            get => this.m_match.Classification;
            set { }
        }

        /// <summary>
        /// Gets or sets the properties that matched and their score
        /// </summary>
        [XmlArray("vectors"), XmlArrayItem("v"), JsonProperty("vector")]
        public List<VectorResultReport> Vectors
        {
            get => this.m_match.Vectors.Select(o => new VectorResultReport(o)).ToList();
            set { }
        }


    }
}
