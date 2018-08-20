using System;
using System.Threading.Tasks;

namespace Model
{
	public enum NetworkProtocol
	{
		TCP,
		UDP
	}

	public abstract class AService: IDisposable
	{
		public abstract AChannel GetChannel(long id);

		public abstract Task<AChannel> AcceptChannel();

		public abstract AChannel ConnectChannel(string host, int port, System.Action<string,System.Net.Sockets.SocketError> callback);

		public abstract void Remove(long channelId);

		public abstract void Update();

		public abstract void Dispose();
	}
}