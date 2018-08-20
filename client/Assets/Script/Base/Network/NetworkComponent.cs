using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model;
namespace Base
{
    public abstract class NetworkComponent : Component, IAwake<NetworkProtocol>, IAwake<NetworkProtocol, string, int>, IUpdate
    {
        private AService Service;

        private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

        public void Awake(NetworkProtocol protocol)
        {
            switch (protocol)
            {
                case NetworkProtocol.TCP:
                    this.Service = new TService();
                    break;
                case NetworkProtocol.UDP:
                    throw new Exception();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Awake(NetworkProtocol protocol, string host, int port)
        {
            switch (protocol)
            {
                case NetworkProtocol.TCP:
                    this.Service = new TService(host, port);
                    break;
                case NetworkProtocol.UDP:
                    throw new Exception();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.StartAccept();
        }

        private async void StartAccept()
        {
            while (true)
            {
                if (this.IsDisposer)
                {
                    return;
                }

                await this.Accept();
            }
        }

        private async Task<Session> Accept()
        {
            AChannel channel = await this.Service.AcceptChannel();
            Session session = new Session(this, channel);
            channel.ErrorCallback += (c, e) => { this.Remove(session); };
            AddSession(session);
            return session;
        }
        public virtual void AddSession(Session session)
        {
            this.sessions.Add(session.Id, session);
        }
        public virtual void Remove(Session session)
        {
            if(this.sessions.Remove(session.Id))
                session.Dispose();
        }

        public Session Get(long id)
        {
            Session session;
            this.sessions.TryGetValue(id, out session);
            return session;
        }

        /// <summary>
        /// 创建一个新Session
        /// </summary>
        public Session Create(string address, System.Action<string, System.Net.Sockets.SocketError> callbac)
        {
            string[] ss = address.Split(':');
            int port = int.Parse(ss[1]);
            string host = ss[0];
            return Create(host, port, callbac);
        }
        public Session Create(string host, int port, System.Action<string, System.Net.Sockets.SocketError> callback)
        {
            AChannel channel = this.Service.ConnectChannel(host, port, callback);
            Session session = new Session(this, channel);
            channel.ErrorCallback += (c, e) => { this.Remove(session); };
            this.sessions.Add(session.Id, session);
            return session;
        }
        public void Update()
        {
            if (this.Service == null)
            {
                return;
            }
            this.Service.Update();
        }

        public override void Dispose()
        {
            if (this.IsDisposer)
            {
                return;
            }

            base.Dispose();

            foreach (Session session in this.sessions.Values.ToArray())
            {
                session.Dispose();
            }

            this.Service.Dispose();
        }
    }
}
