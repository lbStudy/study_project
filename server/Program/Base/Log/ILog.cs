using NLog;

namespace Base
{
	public interface ILog
	{
		void Warning(string message);
		void Info(LogDatabase ldb, LogEventInfo message);
		void Debug(string message);
		void Error(string message);

	}
}