﻿/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;
using SanteDB.OrmLite.Providers.Postgres;

namespace SanteDB.Matcher.Orm.FirebirdSQL
{
    /// <summary>
    /// Represents the PostgreSQL approx function driven by the server configuration
    /// </summary>
    /// <example>
    /// ?name.component.value=:(approx|Betty)
    /// </example>
    public class FirebirdApproxlikeFunction : IDbFilterFunction
    {

        /// <summary>
        /// Tracer to trace the approx function
        /// </summary>
        private Tracer m_tracer = Tracer.GetTracer(typeof(FirebirdApproxlikeFunction));

        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name => "approx";

        /// <summary>
        /// Provider 
        /// </summary>
        public string Provider => "FirebirdSQL";

        /// <summary>
        /// Creates the SQL statement
        /// </summary>
        public SqlStatement CreateSqlStatement(SqlStatement current, string filterColumn, string[] parms, string operand, Type operandType)
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

            var filter = new SqlStatement(new PostgreSQLProvider());
            foreach (var alg in config.ApproxSearchOptions.Where(o => o.Enabled))
            {
                if (alg is ApproxPatternOption pattern)
                {
                    if(pattern.IgnoreCase)
                        filter.Or($"LOWER({filterColumn}) like LOWER(?)",  QueryBuilder.CreateParameterValue(parms[0].Replace("*", "%").Replace("?", "_"), typeof(String)));
                    else
                        filter.Or($"{filterColumn} like ?", QueryBuilder.CreateParameterValue(parms[0].Replace("*", "%").Replace("?", "_"), typeof(String)));

                }
            }

            return current.Append("(").Append(filter).Append(")");

        }
    }

}
