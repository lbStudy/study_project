using NLog;
using System.Collections.Generic;

namespace Base
{
    public enum LogDatabase
    {
        fengyuncard
    }

    public class NLogAdapter: ALogDecorater, ILog
	{
		private readonly Logger logger = LogManager.GetCurrentClassLogger();
        Dictionary<LogDatabase, Logger> loggerdbDic = new Dictionary<LogDatabase, Logger>();
		public NLogAdapter(ALogDecorater decorater = null): base(decorater)
		{
            foreach(LogDatabase v in System.Enum.GetValues(typeof(LogDatabase)))
            {
                Logger logger = LogManager.GetLogger(v.ToString());
                if(logger != null)
                {
                    loggerdbDic[v] = logger;
                }
            }
		}

		public void Warning(string message)
		{
			this.logger.Warn(this.Decorate(message));
		}

		public void Info(LogDatabase ldb, LogEventInfo message)
		{
            Logger logger = null;
            if(loggerdbDic.TryGetValue(ldb, out logger))
            {
                logger.Info(message);
            }
		}

		public void Debug(string message)
		{
			this.logger.Debug(this.Decorate(message));
		}

		public void Error(string message)
		{
			this.logger.Error(this.Decorate(message));
		}
	}
}