/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;
using SanteDB.OrmLite.Providers.Postgres;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SanteDB.Matcher.Orm.PostgreSQL
{
    /// <summary>
    /// Represents the PostgreSQL soundex function
    /// </summary>
    /// <example>
    /// ?name.component.value=:(similarity)Fyfe
    /// or with levenshtein
    /// ?name.component.value=:(similarity|Fyfe)&lt;3
    /// </example>
    [ExcludeFromCodeCoverage]
    public class PostgresSimilarityFunction : IDbFilterFunction
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name => "similarity";

        /// <summary>
        /// Provider 
        /// </summary>
        public string Provider => PostgreSQLProvider.InvariantName;


        /// <summary>
        /// Creates the SQL statement
        /// </summary>
        public SqlStatementBuilder CreateSqlStatement(SqlStatementBuilder current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            var match = new Regex(@"^([<>]?=?)(.*?)$").Match(operand);
            String op = match.Groups[1].Value, value = match.Groups[2].Value;
            if (String.IsNullOrEmpty(op))
            {
                op = "=";
            }

            switch (parms.Length)
            {
                case 0:
                    return current.Append($"{filterColumn} % ?", QueryBuilder.CreateParameterValue(parms[0], operandType));
                case 1: // with levenshtein
                    return current.Append($"{filterColumn} % ? AND similarity({filterColumn}, ?) {op} ?", QueryBuilder.CreateParameterValue(parms[0], operandType), QueryBuilder.CreateParameterValue(parms[0], operandType), QueryBuilder.CreateParameterValue(value, typeof(double)));
                default:
                    throw new ArgumentOutOfRangeException("Invalid number of parameters of string diff");
            }
        }

    }

}
