using System;

namespace Base
{
	public static class TimeHelper
	{
		public static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public const int TotalSecOfDay = 60 * 60 * 24;
        public static long ClientNow()
		{
			return Convert.ToInt64((DateTime.UtcNow - epoch).TotalMilliseconds);
		}

		public static long ClientNowSeconds()
		{
			return Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
		}

		public static long ClientNowTicks()
		{	
			return Convert.ToInt64((DateTime.UtcNow - epoch).Ticks);
		}
        //sec时间戳（秒）
        public static int TotalDays(long sec)
        {
            return (int)(sec / TotalSecOfDay);
            //return (epoch.AddSeconds(sec) - epoch).Days;
        }
    }
}