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
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SanteDB.Matcher.Orm.PostgreSQL
{
    /// <summary>
    /// Postgrsql string difference function
    /// </summary>
    public class PostgresLevenshteinFunction : IDbFilterFunction
    {
        /// <summary>
        /// Gets thje provider name
        /// </summary>
        public string Provider => "pgsql";

        /// <summary>
        /// Gets the name of the filter
        /// </summary>
        public string Name => "similarity_to";

        /// <summary>
        /// Apply the filter
        /// </summary>
        public SqlStatement CreateSqlStatement(SqlStatement current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            var match = new Regex(@"^([<>]?=?)(.*?)$").Match(operand);
            String op = match.Groups[1].Value, value = match.Groups[2].Value;
            if (String.IsNullOrEmpty(op)) op = "=";

            switch (parms.Length)
            {
                case 1:
                    return current.Append($"levenshtein(TRIM(LOWER({filterColumn})), TRIM(LOWER(?))) {op} ?", QueryBuilder.CreateParameterValue(parms[0], operandType), QueryBuilder.CreateParameterValue(value, typeof(Int32)));
                case 4: // with insert, delete and substitute costs
                    return current.Append($"levenshtein(TRIM(LOWER({filterColumn})), TRIM(LOWER(?)), {String.Join(",", parms.Skip(1))}) {op} ?", QueryBuilder.CreateParameterValue(parms[0], operandType), QueryBuilder.CreateParameterValue(value, typeof(Int32)));
                default:
                    throw new ArgumentOutOfRangeException("Invalid number of parameters of string diff");
            }
        }
    }
}
