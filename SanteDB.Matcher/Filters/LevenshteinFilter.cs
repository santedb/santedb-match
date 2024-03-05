/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.Matcher.Util;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SanteDB.Matcher.Filters
{
    /// <summary>
    /// Represents the levenshtein filter
    /// </summary>
    public class LevenshteinFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter 
        /// </summary>
        public string Name => "levenshtein";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(StringDifference).GetRuntimeMethod(nameof(StringDifference.Levenshtein), new Type[] { typeof(String), typeof(String) });

        /// <summary>
        /// Compose the function
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if (parms.Length == 0)
            {
                throw new ArgumentOutOfRangeException("levenshtein requires one parameter : value=:(levenshtein|other)comparator");
            }

            return Expression.MakeBinary(comparison,
                                Expression.Call(this.ExtensionMethod, scope, parms[0]),
                                valueExpression);
        }
    }

    /// <summary>
    /// Represents the levenshtein filter
    /// </summary>
    public class LevenshteinSimilarityFilter : IQueryFilterExtension
    {
        /// <summary>
        /// Gets the name of the filter 
        /// </summary>
        public string Name => "similarity_lev";

        /// <summary>
        /// Gets the extension method
        /// </summary>
        public MethodInfo ExtensionMethod => typeof(StringDifference).GetRuntimeMethod(nameof(StringDifference.LevenshteinSimilarityTo), new Type[] { typeof(String), typeof(String) });

        /// <summary>
        /// Compose the function
        /// </summary>
        public BinaryExpression Compose(Expression scope, ExpressionType comparison, Expression valueExpression, Expression[] parms)
        {
            if (parms.Length == 0)
            {
                throw new ArgumentOutOfRangeException("similarity_lev requires one parameter : value=:(similarity_lev|other)comparator");
            }

            return Expression.MakeBinary(comparison,
                                Expression.Call(this.ExtensionMethod, scope, parms[0]),
                                valueExpression);
        }
    }
}
