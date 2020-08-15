using System;
using System.Collections.Generic;
using System.Text;
using SanteDB.OrmLite;
using SanteDB.OrmLite.Providers;

namespace SanteDB.Matcher.Orm.PostgreSQL
{
    /// <summary>
    /// Represents the PostgreSQL soundex function
    /// </summary>
    /// <example>
    /// ?name.component.value=:(soundslike|Betty)
    /// ?name.component.value=:(soundslike|Betty,metaphone)
    /// </example>
    public class PostgresSoundslikeFunction : IDbFilterFunction
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name => "soundslike";

        /// <summary>
        /// Provider 
        /// </summary>
        public string Provider => "pgsql";

        /// <summary>
        /// Creates the SQL statement
        /// </summary>
        public SqlStatement CreateSqlStatement(SqlStatement current, string filterColumn, string[] parms, string operand, Type operandType)
        {
            if (parms.Length == 1)
                return current.Append($"metaphone({filterColumn}, 4) = metaphone(?, 4)", QueryBuilder.CreateParameterValue(parms[0], operandType));
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
