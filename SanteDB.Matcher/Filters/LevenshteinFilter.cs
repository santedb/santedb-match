using SanteDB.Core.Model.Query;
using SanteDB.Matcher.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Filters
{
    /// <summary>
    /// Represents the levenshtein filter
    /// </summary>
    public class LevenshteinFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter 
        /// </summary>
        public string Name => "levenshtein";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(StringDifference).GetRuntimeMethod(nameof(StringDifference.Levenshtein), new Type[] { typeof(String) });

        /// <summary>
        /// Compose the function
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if (parms.Length != 1) throw new ArgumentOutOfRangeException("levenshtein requires one parameter : value=:(levenshtein|other)comparator");
            return Expression.MakeBinary(comparison,
                                Expression.Call(this.ExtensionMethod, scope, parms[0]),
                                valueExpression);
        }
    }
}
