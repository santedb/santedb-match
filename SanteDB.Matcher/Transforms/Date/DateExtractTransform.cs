/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using System;
using System.Globalization;

namespace SanteDB.Matcher.Transforms.Date
{
    /// <summary>
    /// Extracts a part from the date
    /// </summary>
    public class DateExtractTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Get the name of the transform
        /// </summary>
        public string Name => "date_extract";

        /// <summary>
        /// Apply the transform
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            // Validate parms
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (parms.Length != 1) throw new ArgumentOutOfRangeException(nameof(parms), "Missing facet type");

            // Get strong input
            if (input is DateTimeOffset || input is DateTimeOffset?)
                input = ((DateTimeOffset)input).DateTime;
            var data = (DateTime)input;
            bool epoch = parms.Length == 2 && parms[1].Equals(true);

            // Parameters
            switch (parms[0].ToString())
            {
                case "y":
                    return data.Year;
                case "M":
                    return data.Month;
                case "d":
                    return data.Day;
                case "D":
                    return data.DayOfWeek;
                case "w":
                    return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(data, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);
                case "q":
                    return (int)(data.Month / 4) + 1;
                case "S":
                    return (int)(data.Month / 6) + 1;
                case "h":
                    return data.Hour;
                case "m":
                    return data.Minute;
                case "s":
                    return data.Second;
                case "t":
                    return data.Ticks;
                default:
                    throw new InvalidOperationException($"Don't understand date part {parms[0]}");
            }
        }
    }
}
