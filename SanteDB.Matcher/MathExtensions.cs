using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher
{
    /// <summary>
    /// Math extensoins
    /// </summary>
    public static class MathExtensions
    {

        /// <summary>
        /// The LN function (1/log(me))
        /// </summary>
        public static double Ln (this double me)
        {
            return Math.Log(me, Math.E);
        }
    }
}
