﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;

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
        public string Name => "levenshtein";

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
