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
    /// Query filter for first n characters
    /// </summary>
    public class SubstringFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name => "substr";

        /// <summary>
        /// The extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Substr), new Type[] { typeof(String), typeof(Int32) });

        /// <summary>
        /// Compose the expression
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            for (int i = 0; i < parms.Length; i++) 
                if(parms[i] is ConstantExpression)
                    parms[i] = Expression.Constant(Int32.Parse((parms[i] as ConstantExpression).Value.ToString()));

            if (parms.Length == 1)
                return Expression.MakeBinary(comparison,
                    Expression.Call(this.ExtensionMethod, scope, parms[0]),
                    Expression.Call(this.ExtensionMethod, valueExpression, parms[0]));
            else if (parms.Length == 2)
            {
                var exm = typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Substr), new Type[] { typeof(String), typeof(Int32), typeof(Int32) });
                return Expression.MakeBinary(comparison,
                    Expression.Call(exm, scope, parms[0], parms[1]),
                    Expression.Call(exm, valueExpression, parms[0], parms[1]));
            }
            else
                throw new ArgumentOutOfRangeException($"Invalid number of arguments to substr");
        }
    }
}
