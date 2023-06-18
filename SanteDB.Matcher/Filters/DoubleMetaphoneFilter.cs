/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-5-19
 */
using SanteDB.Core.Model.Query;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SanteDB.Matcher.Filters
{
    /// <summary>
    /// The dmetaphone query filter
    /// </summary>
    public class DoubleMetaphoneFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "dmetaphone";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(FilterExtensionMethods).GetRuntimeMethod(nameof(FilterExtensionMethods.DoubleMetaphone), new Type[] { typeof(String) });

        /// <summary>
        /// Compose the LINQ expression
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            return Expression.MakeBinary(ExpressionType.Equal,
                Expression.Call(this.ExtensionMethod, scope),
                Expression.Call(this.ExtensionMethod, valueExpression));
        }
    }
}
