using SanteDB.Core.Model;
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
    /// Represents a filter that can diff serveral types of data
    /// </summary>
    public class DateDiffFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "date_diff";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Difference), new Type[] { typeof(DateTime), typeof(DateTime) });

        /// <summary>
        /// Compose the function 
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if (parms.Length != 1) throw new ArgumentOutOfRangeException("date_diff requires one parameter : value=:(date_diff|other)comparator");

            // Is scope nullable?
            if (scope.Type.IsConstructedGenericType && scope.Type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                scope = Expression.MakeMemberAccess(scope, scope.Type.GetRuntimeProperty("Value"));

            if (scope.Type.StripNullable() == typeof(DateTimeOffset)) {
                var exm = typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Difference), new Type[] { typeof(DateTimeOffset), typeof(DateTimeOffset) });
                return Expression.MakeBinary(comparison,
                    Expression.Call(exm, scope, parms[0]),
                    valueExpression);
            }
            else
                return Expression.MakeBinary(comparison,
                    Expression.Call(this.ExtensionMethod, scope, parms[0]),
                    valueExpression);
        }
    }
}
