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
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;
using SanteDB.OrmLite.Providers.Sqlite;
using System;

namespace SanteDB.Matcher.Orm.Sqlite
{
    /// <summary>
    /// Soundex filter function
    /// </summary>
    public class SqliteSoundslikeFunction : IDbFilterFunction
    {
        /// <summary>
        /// Get the name of the function
        /// </summary>
        public string Name => "soundslike";

        /// <summary>
        /// Get the provider name
        /// </summary>
        public string Provider => SqliteProvider.InvariantName;

        /// <summary>
        /// Create SQL statement
        /// </summary>
        public SqlStatementBuilder CreateSqlStatement(SqlStatementBuilder current, string filterColumn, string[] parms, string operand, Type operandType)
        {

            if (parms.Length == 1)
            {
                return current.Append($"soundex({filterColumn}) = soundex(?)", QueryBuilder.CreateParameterValue(parms[0], operandType));
            }
            else
            {
                switch (parms[1])
                {
                    case "soundex":
                        return current.Append($"soundex({filterColumn}) = soundex(?)", QueryBuilder.CreateParameterValue(parms[0], operandType));
                    default:
                        throw new NotSupportedException($"Sounds-like algorithm {parms[1]} is not supported");
                }
            }
        }

    }
}
