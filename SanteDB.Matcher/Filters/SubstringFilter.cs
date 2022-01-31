/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-27
 */
using SanteDB.Core.Model.Query;
using System;
using System.Linq.Expressions;
using System.Reflection;

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
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Substr), new Type[] { typeof(String), typeof(Int32), typeof(Int32?) });

        /// <summary>
        /// Compose the expression
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            for (int i = 0; i < parms.Length; i++)
                if (parms[i] is ConstantExpression)
                    parms[i] = Expression.Constant(Int32.Parse((parms[i] as ConstantExpression).Value.ToString()));

            if (parms.Length == 1)
                return Expression.MakeBinary(comparison,
                    Expression.Call(this.ExtensionMethod, scope, parms[0], Expression.Constant(null)),
                    Expression.Call(this.ExtensionMethod, valueExpression, parms[0], Expression.Constant(null)));
            else if (parms.Length == 2)
            {
                var exm = typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.Substr), new Type[] { typeof(String), typeof(Int32), typeof(Int32) });
                return Expression.MakeBinary(comparison,
                    Expression.Call(exm, scope, parms[0], Expression.Convert(parms[1], typeof(Int32?))),
                    Expression.Call(exm, valueExpression, parms[0], Expression.Convert(parms[1], typeof(Int32?))));
            }
            else
                throw new ArgumentOutOfRangeException($"Invalid number of arguments to substr");
        }
    }
}
