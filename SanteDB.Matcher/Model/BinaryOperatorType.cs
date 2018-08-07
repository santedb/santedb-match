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
