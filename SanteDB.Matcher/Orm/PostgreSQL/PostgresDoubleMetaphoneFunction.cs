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
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;
using System;
using System.Text.RegularExpressions;

namespace SanteDB.Matcher.Orm.PostgreSQL
{
    /// <summary>
    /// Represents the PostgreSQL soundex function
    /// </summary>
    public class PostgresDoubleMetaphoneFunction : IDbFilterFunction, IDbIndexFunction
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name => "dmetaphone";

        /// <summary>
        /// Provider 
        /// </summary>
        public string Provider => "pgsql";

        /// <inheritdoc/>
        public SqlStatement CreateIndex(String indexName, String tableName, String column)
        {
            // Create the index
            return new SqlStatement("CREATE INDEX ").Append(indexName).Append(" ON ").Append(tableName).Append($" USING BTREE (DMETAPHONE({column})::TEXT)");
        }

        /// <summary>
        /// Creates the SQL statement
        /// </summary>
        public SqlStatement CreateSqlStatement(SqlStatement current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            var match = new Regex(@"^([<>]?=?)(.*?)$").Match(operand);
            String op = match.Groups[1].Value, value = match.Groups[2].Value;
            if (String.IsNullOrEmpty(op)) op = "=";
            return current.Append($"((dmetaphone({filterColumn}) {op} dmetaphone(?)) OR (dmetaphone_alt({filterColumn}) {op} dmetaphone_alt(?)))", QueryBuilder.CreateParameterValue(value, operandType));
        }
    }

}
