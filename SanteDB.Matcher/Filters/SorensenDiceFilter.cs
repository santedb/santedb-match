using SanteDB.Core.Model.Query;
using SanteDB.Matcher.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SanteDB.Matcher.Filters
{
    /// <summary>
    /// Represents a sorensen-dice extended query filter
    /// </summary>
    public class SorensenDiceFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter 
        /// </summary>
        public string Name => "sorensen_dice";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(StringDifference).GetRuntimeMethod(nameof(StringDifference.JaroWinkler), new Type[] { typeof(String), typeof(String) });

        /// <summary>
        /// Compose the function
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if (parms.Length != 1) throw new ArgumentOutOfRangeException("jaro-winkler requires one parameter : value=:(jarowinkler|other)comparator");
            return Expression.MakeBinary(comparison,
                                Expression.Call(this.ExtensionMethod, scope, parms[0]),
                                valueExpression);
        }
    }
}
