/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using SanteDB.Core.Model.Query;
using System;
using System.Linq.Expressions;
using System.Reflection;

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
            {
                return Expression.MakeBinary(ExpressionType.Equal,
                    Expression.Call(this.ExtensionMethod, scope, Expression.Convert(parms[0], typeof(String))),
                    Expression.Constant(true));
            }
            else if (parms.Length == 2)
            {
                return Expression.MakeBinary(ExpressionType.Equal,
                    Expression.Call(this.ExtensionMethod, scope, Expression.Convert(parms[0], typeof(String)), Expression.Convert(parms[1], typeof(string))),
                    Expression.Constant(true));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid number of arguments to the soundslike function.");
            }
        }
    }
}
