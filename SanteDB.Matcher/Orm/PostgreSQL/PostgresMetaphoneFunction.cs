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
    [ExcludeFromCodeCoverage]
    public class PostgresMetaphoneFunction : IDbFilterFunction
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name => "metaphone";

        /// <summary>
        /// Provider 
        /// </summary>
        public string Provider => PostgreSQLProvider.InvariantName;

        /// <summary>
        /// Creates the SQL statement
        /// </summary>
        /// <example>
        /// ?name.component.value=:(metaphone)Justin
        /// or
        /// ?name.component.value=:(metaphone|5)Hamilton
        /// </example>
        public SqlStatementBuilder CreateSqlStatement(SqlStatementBuilder current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            var match = new Regex(@"^([<>]?=?)(.*?)$").Match(operand);
            String op = match.Groups[1].Value, value = match.Groups[2].Value;
            if (String.IsNullOrEmpty(op))
            {
                op = "=";
            }

            if (op != "=") // There is a threshold
            {
                return current.Append($"metaphone({filterColumn}, {parms[0]}) {op} metaphone(?, {parms[0]})", QueryBuilder.CreateParameterValue(value, operandType));
            }
            else
            {
                return current.Append($"metaphone({filterColumn}, 4) {op} metaphone(?, 4)", QueryBuilder.CreateParameterValue(value, operandType));
            }
        }
    }
}
