/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using System;
using System.ComponentModel;

namespace SanteDB.Matcher.Transforms.Timespan
{
    /// <summary>
    /// Represents an extract transform that is for timespan
    /// </summary>
    [Description("Extract a part of a timespan"), DisplayName("Timespan Extract")]
    [TransformArgument(typeof(String), "part", "y = Years, M = Months, d = Days, w = Weeks, q = Quarters, h = Hours, m = Minutes, s = Seconds")]
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
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (parms.Length != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(parms), "Missing facet type");
            }

            var data = (TimeSpan)input;
            switch (parms[0].ToString())
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