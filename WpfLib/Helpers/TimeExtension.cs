using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfLib.Helpers
{
    public static class TimeExtension
    {
        public static long TimeStamp(this DateTime time)
        {
            DateTime dd = new (1970, 1, 1, 0, 0, 0, 0);
            DateTime timeUtc = DateTime.SpecifyKind(time, DateTimeKind.Utc);
            TimeSpan ts = (timeUtc - dd);
            return (long)ts.TotalMilliseconds;
        }
    }
}
