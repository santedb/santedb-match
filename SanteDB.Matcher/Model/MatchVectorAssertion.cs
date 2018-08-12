using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Match vector rule
    /// </summary>
    [XmlType(nameof(MatchVectorAssertion), Namespace = "http://santedb.org/matcher")]
    public class MatchVectorAssertion
    {

        /// <summary>
        /// Operator for classifier
        /// </summary>
        [XmlAttribute("op")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the value of the comparator
        /// </summary>
        [XmlAttribute("value")]
        public double Value { get; set; }

        /// <summary>
        /// Value specified
        /// </summary>
        [XmlIgnore]
        public bool ValueSpecified { get; set; }

        /// <summary>
        /// Gets or sets the transformations
        /// </summary>
        [XmlElement("transform")]
        public List<MatchTransform> Transforms { get; set; }

        /// <summary>
        /// Rules in the vector rule
        /// </summary>
        [XmlElement("assert")]
        public List<MatchVectorAssertion> Assertions { get; set; }

    }
}