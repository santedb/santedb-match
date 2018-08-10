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
