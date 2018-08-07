using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.Matcher.Model
{
    /// <summary>
    /// Represents behaviors when a vector property is null
    /// </summary>
    [XmlType(nameof(MatchVectorNullBehavior), Namespace = "http://santedb.org/matcher")]
    public enum MatchVectorNullBehavior
    {
        /// <summary>
        /// When the field is null on the queried record apply the mWeight
        /// </summary>
        [XmlEnum("match")]
        Match,
        /// <summary>
        /// When the property is null on the queried record apply the uWeight
        /// </summary>
        [XmlEnum("nonmatch")]
        NonMatch,
        /// <summary>
        /// When the property is null on the queried record ignore the rule
        /// </summary>
        [XmlEnum("ignore")]
        Ignore,
        /// <summary>
        /// When the property is null on the queried record, disqualify the match entirely
        /// </summary>
        [XmlEnum("disqualify")]
        Disqualify

    }
}
