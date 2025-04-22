using System;
using System.Globalization;

namespace IXR
{
    /// <summary>
    /// Represents a date and time in ISO 8601 format for XML-RPC.
    /// </summary>
    public class IXR_Date
    {
        // Properties for date and time components
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }
        public string Timezone { get; private set; }

        /// <summary>
        /// Constructor to initialize the IXR_Date object.
        /// </summary>
        /// <param name="time">A timestamp (numeric) or an ISO 8601 string.</param>
        public IXR_Date(object time)
        {
            if (time == null)
                throw new ArgumentNullException(nameof(time), "Time cannot be null.");

            if (time is long timestamp)
            {
                ParseTimestamp(timestamp);
            }
            else if (time is string iso)
            {
                ParseIso(iso);
            }
            else
            {
                throw new ArgumentException("Invalid time format. Expected numeric timestamp or ISO 8601 string.");
            }
        }

        /// <summary>
        /// Parses a Unix timestamp into date and time components.
        /// </summary>
        /// <param name="timestamp">The Unix timestamp.</param>
        private void ParseTimestamp(long timestamp)
        {
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            Year = dateTime.Year;
            Month = dateTime.Month;
            Day = dateTime.Day;
            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
            Timezone = ""; // No timezone for Unix timestamps
        }

        /// <summary>
        /// Parses an ISO 8601 string into date and time components.
        /// </summary>
        /// <param name="iso">The ISO 8601 string.</param>
        private void ParseIso(string iso)
        {
            if (string.IsNullOrEmpty(iso))
                throw new ArgumentException("ISO string cannot be null or empty.");

            try
            {
                // Extract date and time components
                Year = int.Parse(iso.Substring(0, 4));
                Month = int.Parse(iso.Substring(4, 2));
                Day = int.Parse(iso.Substring(6, 2));
                Hour = int.Parse(iso.Substring(9, 2));
                Minute = int.Parse(iso.Substring(12, 2));
                Second = int.Parse(iso.Substring(15, 2));

                // Extract timezone (if any)
                Timezone = iso.Length > 17 ? iso.Substring(17) : "";
            }
            catch
            {
                throw new FormatException("Invalid ISO 8601 format.");
            }
        }

        /// <summary>
        /// Gets the ISO 8601 representation of the date and time.
        /// </summary>
        /// <returns>An ISO 8601 formatted string.</returns>
        public string GetIso()
        {
            return $"{Year:D4}{Month:D2}{Day:D2}T{Hour:D2}:{Minute:D2}:{Second:D2}{Timezone}";
        }

        /// <summary>
        /// Gets the XML representation of the date and time.
        /// </summary>
        /// <returns>An XML string with the ISO 8601 date and time.</returns>
        public string GetXml()
        {
            return $"<dateTime.iso8601>{GetIso()}</dateTime.iso8601>";
        }

        /// <summary>
        /// Gets the Unix timestamp representation of the date and time.
        /// </summary>
        /// <returns>A Unix timestamp as a long integer.</returns>
        public long GetTimestamp()
        {
            var dateTime = new DateTime(Year, Month, Day, Hour, Minute, Second, DateTimeKind.Utc);
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }
    }
}