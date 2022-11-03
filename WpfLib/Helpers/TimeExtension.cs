using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfLib.Helpers
{
    public static class TimeExtension
    {
        private static readonly DateTime Init = new(1970, 1, 1, 0, 0, 0, 0);
        public static long TimeStamp(this DateTime time)
        {
            return (long)(DateTime.SpecifyKind(time, DateTimeKind.Local) - Init).TotalMilliseconds;
        }
    }
}
