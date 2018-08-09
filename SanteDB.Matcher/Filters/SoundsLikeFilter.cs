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
    /// The "Sounds Like" filter
    /// </summary>
    public class SoundsLike : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "soundslike";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.SoundsLike), new Type[] { typeof(String), typeof(String) });

        /// <summary>
        /// Compose the LINQ expression
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {

            if (parms.Length == 1)
                return Expression.MakeBinary(ExpressionType.Equal,
                    Expression.Call(this.ExtensionMethod, scope, parms[0]),
                    Expression.Constant(true));
            else if (parms.Length == 2)
                return Expression.MakeBinary(ExpressionType.Equal,
                    Expression.Call(this.ExtensionMethod, scope, parms[0], parms[1]),
                    Expression.Constant(true));
            else
                throw new ArgumentOutOfRangeException("Invalid number of arguments to the soundslike function.");
        }
    }
}
