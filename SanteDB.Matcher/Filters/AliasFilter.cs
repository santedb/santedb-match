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
    /// Represents the aliasing filter
    /// </summary>
    public class AliasFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "alias";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Alias), new Type[] { typeof(String), typeof(String) });

        /// <summary>
        /// Compose the LINQ statemnet
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if (valueExpression == null)
            {
                valueExpression = Expression.Constant(1.0d);
                comparison = ExpressionType.Equal;
            }

            if (parms.Length == 1)
                return Expression.MakeBinary(comparison,
                    Expression.Call(this.ExtensionMethod, scope, parms[0]),
                    valueExpression);
            else
                throw new ArgumentOutOfRangeException($"Invalid number of parameters to alias function");
        }
    }
}
