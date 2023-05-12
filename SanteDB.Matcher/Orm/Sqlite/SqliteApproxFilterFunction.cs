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
 * Date: 2023-3-10
 */
using SanteDB.Core;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;
using SanteDB.OrmLite.Providers.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SanteDB.Matcher.Orm.Sqlite
{
    /// <summary>
    /// Approximate filter function
    /// </summary>
    public class SqliteApproxFilterFunction : IDbInitializedFilterFunction
    {

        // Has spellfix?
        private static bool? m_hasSpellFix;

        // Has soundex?
        private static bool? m_hasSoundex;

        /// <summary>
        /// Name of the filter function
        /// </summary>
        public string Name => "approx";

        /// <summary>
        /// Get the provider
        /// </summary>
        public string Provider => SqliteProvider.InvariantName;

        /// <summary>
        /// Create SQL statement
        /// </summary>
        public SqlStatementBuilder CreateSqlStatement(SqlStatementBuilder current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            if (parms.Length != 1)
                throw new ArgumentException("Approx requires at least one parameter");

            var config = ApplicationServiceContext.Current.GetService<IConfigurationManager>().GetSection<ApproximateMatchingConfigurationSection>();
            if (config == null)
                config = new ApproximateMatchingConfigurationSection()
                {
                    ApproxSearchOptions = new List<ApproxSearchOption>()
                    {
                        new ApproxPatternOption() { Enabled = true, IgnoreCase = true }
                    }
                };

            var filter = new SqlStatementBuilder(current.DbProvider);

            foreach (var alg in config.ApproxSearchOptions.Where(o => o.Enabled))
            {
                if (alg is ApproxDifferenceOption difference && m_hasSpellFix.GetValueOrDefault())
                    filter.Or($"(length(trim({filterColumn})) > {difference.MaxDifference * 2} AND  editdist3(TRIM(LOWER({filterColumn})), TRIM(LOWER(?))) <= {difference.MaxDifference})", QueryBuilder.CreateParameterValue(parms[0], typeof(String)));
                else if (alg is ApproxPhoneticOption phonetic && m_hasSoundex.GetValueOrDefault())
                {
                    var min = phonetic.MinSimilarity;
                    if (!phonetic.MinSimilaritySpecified) min = 1.0f;
                    if (phonetic.Algorithm == ApproxPhoneticOption.PhoneticAlgorithmType.Soundex || phonetic.Algorithm == ApproxPhoneticOption.PhoneticAlgorithmType.Auto)
                        filter.Or($"((4 - editdist3(soundex({filterColumn}), soundex(?)))/4.0) >= {min}", QueryBuilder.CreateParameterValue(parms[0], typeof(String)));
                    else if (phonetic.Algorithm == ApproxPhoneticOption.PhoneticAlgorithmType.Metaphone)
                        filter.Or($"((length(spellfix1_phonehash({filterColumn})) - editdist3(spellfix1_phonehash({filterColumn}), spellfix1_phonehash(?)))/length(spellfix1_phonehash({filterColumn}))) >= {min}", QueryBuilder.CreateParameterValue(parms[0], typeof(String)));
                    else
                        throw new InvalidOperationException($"Phonetic algorithm {phonetic.Algorithm} is not valid");
                }
                else if (alg is ApproxPatternOption pattern)
                {
                    filter.Or($"({filterColumn} like ?)", QueryBuilder.CreateParameterValue(parms[0].Replace("*", "%").Replace("?", "_"), typeof(String)));
                }
            }

            return current.Append("(").Append(filter).Append(")");
        }

        /// <summary>
        /// True if the extension is installed
        /// </summary>
        public bool Initialize(IDbConnection connection)
        {
            if (!m_hasSoundex.HasValue)
            {
                try
                {
                    m_hasSoundex = connection.ExecuteScalar<Int32>("select sqlite_compileoption_used('SQLITE_SOUNDEX');") == 1;
                    if (connection.ExecuteScalar<Int32>("SELECT sqlite_compileoption_used('SQLITE_ENABLE_LOAD_EXTENSION')") == 1)
                    {
                        try
                        {
                            try
                            {
                                m_hasSpellFix = connection.ExecuteScalar<Int32>("SELECT editdist3('test','test1');") > 0;
                            }
                            catch
                            {
                                connection.LoadExtension("spellfix");
                                m_hasSpellFix = connection.ExecuteScalar<Int32>("SELECT editdist3('test','test1');") > 0;
                            }
                        }
                        catch { m_hasSpellFix = false; }
                    }
                }
                catch
                {
                    m_hasSoundex = false;
                }
            }
            else if (m_hasSpellFix.GetValueOrDefault())
            {
                connection.LoadExtension("spellfix");

            }
            return true;
        }
    }
}
