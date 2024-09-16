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
 */
using SanteDB.Core.Diagnostics;
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;
using SanteDB.OrmLite.Providers.Sqlite;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace SanteDB.Matcher.Orm.Sqlite
{
    /// <summary>
    /// Phonehash (loosely based on Metaphone) filter function
    /// </summary>
    public class SqliteMetaphoneFilterFunction : IDbInitializedFilterFunction
    {
        /// <summary>
        /// Name of the filter function
        /// </summary>
        public string Name => "metaphone";

        /// <summary>
        /// Get the name of the provider
        /// </summary>
        public string Provider => SqliteProvider.InvariantName;

        ///<inheritdoc />
        public int Order => -100;

        /// <summary>
        /// Create SQL statement
        /// </summary>
        public SqlStatementBuilder CreateSqlStatement(SqlStatementBuilder current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            var match = new Regex(@"^([<>]?=?)(.*?)$").Match(operand);
            String op = match.Groups[1].Value, value = match.Groups[2].Value;
            if (String.IsNullOrEmpty(op))
            {
                op = "=";
            }

            if (parms.Length == 1) // There is a threshold
            {
                return current.Append($"editdist3(spellfix1_phonehash({filterColumn}), spellfix1_phonehash(?)) {op} ?", QueryBuilder.CreateParameterValue(parms[0], operandType), QueryBuilder.CreateParameterValue(value, operandType));
            }
            else
            {
                return current.Append($"spellfix1_phonehash({filterColumn}) {op} spellfix1_phonehash(?)", QueryBuilder.CreateParameterValue(value, operandType));
            }
        }

        /// <summary>
        /// True if the extension is installed
        /// </summary>
        public bool Initialize(IDbConnection connection, IDbTransaction transaction) => connection.CheckAndLoadSpellfix();
    }
}
