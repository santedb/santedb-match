using Newtonsoft.Json;
using SanteDB.Matcher.Matchers;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a match vector
    /// </summary>
    [XmlType(nameof(VectorResultReport), Namespace = "http://santedb.org/matcher"), JsonObject]
    public class VectorResultReport
    {

        // Vector result
        private VectorResult m_result;

        /// <summary>
        /// Default ctor for serializer
        /// </summary>
        public VectorResultReport()
        {
        }

        /// <summary>
        /// Creates a new result report from the specified result
        /// </summary>
        public VectorResultReport(VectorResult result)
        {
            this.m_result = result;
        }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name
        {
            get => this.m_result.Name;
            set { }
        }

        /// <summary>
        /// True if the vector was evaluated
        /// </summary>
        [XmlAttribute("evaluated"), JsonProperty("evaluated")]
        public bool Evaluated
        {
            get => this.m_result.Evaluated;
            set { }
        }

        /// <summary>
        /// Gets the configured probability
        /// </summary>
        [XmlAttribute("m"), JsonProperty("m")]
        public double ConfiguredProbability
        {
            get => this.m_result.ConfiguredProbability;
            set { }
        }


        /// <summary>
        /// Gets the configured weight
        /// </summary>
        [XmlAttribute("w"), JsonProperty("w")]
        public double ConfiguredWeight
        {
            get => this.m_result.ConfiguredWeight;
            set { }
        }

        /// <summary>
        /// Gets the score assigned to this vector
        /// </summary>
        [XmlAttribute("score"), JsonProperty("score")]
        public double Score
        {
            get => this.m_result.Score;
            set { }
        }


    }
}