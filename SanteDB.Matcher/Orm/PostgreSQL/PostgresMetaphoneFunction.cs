using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;

namespace SanteDB.Matcher.Orm.PostgreSQL
{
    /// <summary>
    /// Represents the PostgreSQL soundex function
    /// </summary>
    public class PostgresMetaphoneFunction : IDbFilterFunction
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name => "metaphone";

        /// <summary>
        /// Provider 
        /// </summary>
        public string Provider => "pgsql";

        /// <summary>
        /// Creates the SQL statement
        /// </summary>
        /// <example>
        /// ?name.component.value=:(metaphone)Justin
        /// or
        /// ?name.component.value=:(metaphone|5)Hamilton
        /// </example>
        public SqlStatement CreateSqlStatement(SqlStatement current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            var match = new Regex(@"^([<>]?=?)(.*?)$").Match(operand);
            String op = match.Groups[1].Value, value = match.Groups[2].Value;
            if (String.IsNullOrEmpty(op)) op = "=";

            if (op != "=") // There is a threshold
                return current.Append($"metaphone({filterColumn}, {parms[0]}) {op} metaphone(?, {parms[0]})", QueryBuilder.CreateParameterValue(value, operandType));
            else
                return current.Append($"metaphone({filterColumn}, 4) {op} metaphone(?, 4)", QueryBuilder.CreateParameterValue(value, operandType));
        }
    }
}
