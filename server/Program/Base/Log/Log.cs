using NLog;

namespace Base
{
	public static class Log
	{
		private static readonly ILog globalLog = new NLogAdapter();

        public static void Init(AppType appType, int appid)
        {
            LogManager.Configuration.Variables["appType"] = appType.ToString();
            LogManager.Configuration.Variables["appId"] = appid.ToString();
        }

        public static void Warning(string message)
		{
			globalLog.Warning(message);
		}

        public static void Info(LogDatabase ldb,long sid,long role_id,int role_lv,string role_name, string a_type, string action, int r_before, int r_after,int v1 = 0, int v2 = 0, string ext = "")
        {
            LogEventInfo ei = new LogEventInfo(LogLevel.Info,"", "");
            ei.Properties["version"] = "1.0";
            ei.Properties["time"] = TimeHelper.ClientNowSeconds();
            ei.Properties["sid"] = sid;
            ei.Properties["role_id"] = role_id;
            ei.Properties["role_lv"] = role_lv;
            ei.Properties["role_name"] = role_name;
            ei.Properties["a_type"] = a_type.ToString();
            ei.Properties["action"] = action.ToString();
            ei.Properties["r_before"] = r_before;
            ei.Properties["r_after"] = r_after;
            ei.Properties["v1"] = v1;
            ei.Properties["v2"] = v2;
            ei.Properties["ext"] = ext;

            globalLog.Info(ldb, ei);
        }
		public static void Debug(string message)
		{
			globalLog.Debug(message);
		}

		public static void Error(string message)
		{
			globalLog.Error(message);
		}
	}
}
