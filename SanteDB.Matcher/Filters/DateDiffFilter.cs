/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2021-8-5
 */
using SanteDB.Core.Model;
using SanteDB.Core.Model.Query;
using System;
using System.Linq.Expressions;
using System.Reflection;

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
            if (scope.Type.IsConstructedGenericType && scope.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                scope = Expression.MakeMemberAccess(scope, scope.Type.GetRuntimeProperty("Value"));

            // Convert value expression if it is a string
            var constVal = valueExpression as ConstantExpression;
            if (constVal != null && constVal.Value is String)
                valueExpression = Expression.Constant(TimeSpan.Parse(constVal.Value.ToString()));

            // Convert scope 
            constVal = scope as ConstantExpression;
            if (constVal != null && constVal.Value is String)
                scope = Expression.Constant(DateTime.Parse(constVal.Value.ToString()));

            // Convert expression
            if (parms[0].NodeType == ExpressionType.Constant)
            {
                constVal = parms[0] as ConstantExpression;
                if (constVal == null || "null".Equals(constVal.Value.ToString()))
                    parms[0] = Expression.Constant(default(DateTime));
                else if (constVal != null && constVal.Value is String && DateTime.TryParseExact(
                    constVal.Value.ToString(), new string[] { "o", "yyyy", "yyyy-MM", "yyyy-MM-dd" },
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                    out DateTime val))
                    parms[0] = Expression.Constant(val);
            }

            if (scope.Type.StripNullable() == typeof(DateTimeOffset))
            {
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
