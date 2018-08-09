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
    /// The metaphone query filter
    /// </summary>
    public class MetaphoneFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "metaphone";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Metaphone), new Type[] { typeof(String) });

        /// <summary>
        /// Compose the LINQ expression
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if(parms.Length == 0)
                return Expression.MakeBinary(ExpressionType.Equal,
                    Expression.Call(this.ExtensionMethod, scope),
                    Expression.Call(this.ExtensionMethod, valueExpression));
            else 
            {
                var exm = typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Metaphone), new Type[] { typeof(String), typeof(Int32) });
                return Expression.MakeBinary(ExpressionType.Equal,
                    Expression.Call(exm, scope, parms[0]),
                    Expression.Call(exm, valueExpression, parms[0]));
            }
        }
    }
}
