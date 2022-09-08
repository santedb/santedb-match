/*
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
 * Date: 2022-5-30
 */
using System;
using System.ComponentModel;

namespace SanteDB.Matcher.Transforms.Date
{
    /// <summary>
    /// Represents a transform that calculates the difference between the input date and comparison date
    /// </summary>
    [Description("Subtract date A from date B - indicating the desired interval portion (w, d, h, etc.)"), DisplayName("Date Difference")]
    [TransformArgument(typeof(String), "part", "The part of the resulting interval to return (y = Year, M = month, d = Days, w = Weeks, q = Quarters, etc.)", false)]
    [TransformArgument(typeof(Boolean), "absolute", "True if absolute value should be returned", false)]
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
            if (parms.Length == 0)
                return ((DateTime)a - (DateTime)b);
            else
            {
                var data = ((DateTime)a - (DateTime)b);
                double retVal = 0;
                switch (parms[0].ToString())
                {
                    case "y":
                        retVal = ((DateTime)a).Year - ((DateTime)b).Year;
                        break;

                    case "M":
                        retVal = (((DateTime)a).Year * 12 + ((DateTime)a).Month) -
                            (((DateTime)b).Year * 12 + ((DateTime)b).Month);
                        break;

                    case "d":
                        retVal = data.TotalDays;
                        break;

                    case "w":
                        retVal = data.TotalDays / 7;
                        break;

                    case "q":
                        retVal = ((((DateTime)a).Year * 12 + ((DateTime)a).Month) -
                            (((DateTime)b).Year * 12 + ((DateTime)b).Month)) / 4;
                        break;

                    case "h":
                        retVal = data.TotalHours;
                        break;

                    case "m":
                        retVal = data.TotalMinutes;
                        break;

                    case "s":
                        retVal = data.TotalSeconds;
                        break;

                    case "t":
                        retVal = data.Ticks;
                        break;

                    default:
                        throw new InvalidOperationException($"Don't uderstand timespan part {parms[0]}");
                }

                if (parms.Length == 2 && Boolean.TryParse(parms[1].ToString(), out bool abs) && abs)
                    return Math.Abs(retVal);
                else
                    return retVal;
            }
        }
    }
}