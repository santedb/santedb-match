﻿/*
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
using SanteDB.Core.Diagnostics;
using SanteDB.OrmLite.Providers;
using SanteDB.OrmLite.Providers.Sqlite;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using SanteDB.OrmLite;
using System.Data;

namespace SanteDB.Matcher.Orm.Sqlite
{
    /// <summary>
    /// Substring filter function
    /// </summary>
    public class SqliteLevenshteinFilterFunction : IDbInitializedFilterFunction
    {
        /// <summary>
        /// Name of the filter function
        /// </summary>
        public string Name => "levenshtein";

        /// <summary>
        /// Get the invariant name
        /// </summary>
        public string Provider => SqliteProvider.InvariantName;

        /// <summary>
        /// Create SQL statement
        /// </summary>
        public SqlStatement CreateSqlStatement(SqlStatement current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            var match = new Regex(@"^([<>]?=?)(.*?)$").Match(operand);
            String op = match.Groups[1].Value, value = match.Groups[2].Value;
            if (String.IsNullOrEmpty(op)) op = "=";

            switch (parms.Length)
            {
                case 1:
                    return current.Append($"editdist3(TRIM(LOWER({filterColumn})), TRIM(LOWER(?))) {op} ?", QueryBuilder.CreateParameterValue(parms[0], operandType), QueryBuilder.CreateParameterValue(value, typeof(Int32)));
                default:
                    throw new ArgumentOutOfRangeException("Invalid number of parameters of string diff");
            }
        }

        /// <summary>
        /// True if the extension is installed
        /// </summary>
        public bool Initialize(IDbConnection connection)
        {
            if (Assembly.GetEntryAssembly() != null &&
                !String.IsNullOrEmpty(Assembly.GetEntryAssembly().Location) &&
                (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SpellFix.dll")) ||
                File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "spellfix.so"))))
            {
                try
                {
                    if (connection.ExecuteScalar<Int32>("SELECT sqlite_compileoption_used('SQLITE_ENABLE_LOAD_EXTENSION')") == 1)
                    {
                        try // It might already be loaded
                        {
                            connection.ExecuteScalar<Int32>("SELECT editdist3('test','test1');");
                        }
                        catch
                        {
                            connection.LoadExtension("spellfix");
                        }

                        var diff = connection.ExecuteScalar<Int32>("SELECT editdist3('test','test1');");
                        if (diff > 1)
                            connection.ExecuteScalar<Int32>("SELECT editdist3('__sfEditCost');");
                    }
                    return true;
                }
                catch (Exception e) when (e.Message == "SQL logic error")
                {
                    return false;
                }
                catch (Exception e)
                {
                    Tracer.GetTracer(typeof(SqliteLevenshteinFilterFunction)).TraceWarning("Could not initialize SpellFix - {0}", e);
                    return false;
                }

            }
            return false;
        }
    }
}
