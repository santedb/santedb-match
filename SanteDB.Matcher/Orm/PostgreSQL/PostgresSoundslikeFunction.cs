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

namespace SanteDB.Matcher.Orm.PostgreSQL
{
    /// <summary>
    /// Represents the PostgreSQL soundex function
    /// </summary>
    /// <example>
    /// ?name.component.value=:(soundslike|Betty)
    /// ?name.component.value=:(soundslike|Betty,metaphone)
    /// </example>
    [ExcludeFromCodeCoverage]
    public class PostgresSoundslikeFunction : IDbFilterFunction
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name => "soundslike";

        /// <summary>
        /// Provider 
        /// </summary>
        public string Provider => PostgreSQLProvider.InvariantName;

        /// <summary>
        /// Creates the SQL statement
        /// </summary>
        public SqlStatementBuilder CreateSqlStatement(SqlStatementBuilder current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            if (parms.Length == 1)
            {
                return current.Append($"metaphone({filterColumn}, 4) = metaphone(?, 4)", QueryBuilder.CreateParameterValue(parms[0], operandType));
            }
            else
            {
                switch (parms[1])
                {
                    case "metaphone":
                        return current.Append($"metaphone({filterColumn}, 4) = metaphone(?, 4)", QueryBuilder.CreateParameterValue(parms[0], operandType));
                    case "dmetaphone":
                        return current.Append($"((dmetaphone({filterColumn}) = dmetaphone(?)) OR (dmetaphone_alt({filterColumn}) = dmetaphone_alt(?)))", QueryBuilder.CreateParameterValue(parms[0], operandType));
                    case "soundex":
                        return current.Append($"soundex({filterColumn}) = soundex(?)", QueryBuilder.CreateParameterValue(parms[0], operandType));
                    default:
                        throw new NotSupportedException($"Sounds-like algorithm {parms[1]} is not supported");
                }
            }
        }
    }
}
