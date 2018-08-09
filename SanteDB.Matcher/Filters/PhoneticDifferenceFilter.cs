using SanteDB.Core.Model.Query;
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
    /// The "Similar To" filter which returns a levenshtein difference of the sounds like function
    /// </summary>
    public class SimilarTo : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "phonetic_diff";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.PhoneticDifference), new Type[] { typeof(String), typeof(String) });

        /// <summary>
        /// Compose the LINQ expression
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {

            if (parms.Length == 1)
                return Expression.MakeBinary(comparison,
                    Expression.Call(this.ExtensionMethod, scope, parms[0]),
                    valueExpression);
            else if (parms.Length == 2)
                return Expression.MakeBinary(comparison,
                    Expression.Call(this.ExtensionMethod, scope, parms[0], parms[1]),
                    valueExpression);
            else
                throw new ArgumentOutOfRangeException("Invalid number of arguments to the phoneticDifference function.");
        }
    }
}
