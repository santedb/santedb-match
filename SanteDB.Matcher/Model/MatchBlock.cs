using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents a block configuration which are IMSI expressions to send to the database
    /// </summary>
    [XmlType(nameof(MatchBlock), Namespace = "http://santedb.org/matcher")]
    public class MatchBlock
    {

        /// <summary>
        /// Gets or sets the binary operator
        /// </summary>
        [XmlAttribute("op")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the block filters
        /// </summary>
        [XmlElement("imsiExpression")]
        public List<ImsiExpressionFilter> Filter { get; set; }

    }
}
