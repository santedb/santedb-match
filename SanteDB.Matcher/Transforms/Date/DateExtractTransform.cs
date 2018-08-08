using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Name => "date.extract";

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
            switch(parms[0].ToString())
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
