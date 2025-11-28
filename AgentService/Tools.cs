using Azure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentService
{
    public static class Tools
    {
        // [Description("Gets the current date and time based on the specified TimeType.")]
        public static DateTime GetCurrentDateAndTime(TimeType timeType)
        {
            return timeType switch
                {
                TimeType.Local => DateTime.Now,
                TimeType.Utc => DateTime.UtcNow,
                _ => throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null)
            };
        }

        // [Description("Gets the current time zone of the system.")]
        public static string GetCurrentTimeZone()
        {
            return TimeZoneInfo.Local.StandardName;
        }

    }

    public enum TimeType
    {
        Local,
        Utc
    }
}
