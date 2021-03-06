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
 * Date: 2021-2-9
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
                return ((DateTime)a - (DateTime)b);
            else
            {
                var data = ((DateTime)a - (DateTime)b);
                double retVal = 0;
                switch (parms[0].ToString())
                {
                    case "y":
                        retVal =((DateTime)a).Year - ((DateTime)b).Year;
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

                if (parms.Length == 2 && parms[1].Equals(true))
                    return Math.Abs(retVal);
                else
                    return retVal;
            }
        }
    }
}
