/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 *
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
 * User: JustinFyfe
 * Date: 2019-1-22
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Date
{
    /// <summary>
    /// Represents a transform that calculates the difference between the input date and comparison date
    /// </summary>
    public class DateDifferenceTransform : IBinaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "date_diff";

        /// <summary>
        /// Difference between date a and date b
        /// </summary>
        public object Apply(object a, object b, params object[] parms)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            // Get strong instances
            if(parms.Length == 0)
                return ((DateTimeOffset)a - (DateTimeOffset)b);
            else
            {
                var data = ((DateTimeOffset)a - (DateTimeOffset)b);
                switch (parms[0].ToString())
                {
                    case "y":
                        return ((DateTimeOffset)a).Year - ((DateTimeOffset)b).Year;
                    case "M":
                        return (((DateTimeOffset)a).Year * 12 + ((DateTimeOffset)a).Month) -
                            (((DateTimeOffset)b).Year * 12 + ((DateTimeOffset)b).Month);
                    case "d":
                        return data.TotalDays;
                    case "w":
                        return data.TotalDays / 7;
                    case "q":
                        return ((((DateTimeOffset)a).Year * 12 + ((DateTimeOffset)a).Month) -
                            (((DateTimeOffset)b).Year * 12 + ((DateTimeOffset)b).Month)) / 4;
                    case "h":
                        return data.TotalHours;
                    case "m":
                        return data.TotalMinutes;
                    case "s":
                        return data.TotalSeconds;
                    case "t":
                        return data.Ticks;
                    default:
                        throw new InvalidOperationException($"Don't uderstand timespan part {parms[0]}");
                }
            }
        }
    }
}
