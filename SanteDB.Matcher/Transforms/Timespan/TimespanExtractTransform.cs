/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Transforms.Timespan
{
    /// <summary>
    /// Represents an extract transform that is for timespan
    /// </summary>
    public class TimespanExtractTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Gets the name of the transform
        /// </summary>
        public string Name => "timespan_extract";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            // Validate parms
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (parms.Length != 1) throw new ArgumentOutOfRangeException(nameof(parms), "Missing facet type");

            var data = (TimeSpan)input;
            switch(parms[0].ToString())
            {
                case "y":
                    return data.TotalDays / 365;
                case "M":
                    return data.TotalDays / 30;
                case "d":
                    return data.TotalDays;
                case "w":
                    return data.TotalDays / 7;
                case "q":
                    return data.TotalDays / 91; 
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
