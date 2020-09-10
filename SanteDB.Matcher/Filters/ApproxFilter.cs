﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SanteDB.Core.Model.Query;

namespace SanteDB.Matcher.Filters
{
    /// <summary>
    /// Approximate filter
    /// </summary>
    public class ApproxFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "approx";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Approx), new Type[] { typeof(String), typeof(String) });

        /// <summary>
        /// Compose the LINQ expression
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if (parms.Length != 1)
                throw new ArgumentException("Approx requires parameter - use :(approx|value)");
            return Expression.MakeBinary(ExpressionType.Equal,
                Expression.Call(this.ExtensionMethod, scope, parms[0]),
                Expression.Constant(true));
        }
    }
}