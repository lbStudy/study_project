using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Base
{
    public abstract class NetworkComponent : Component, IAwake<NetworkProtocol, AppType>, IAwake<NetworkProtocol, IPEndPoint, AppType>, IUpdate
    {
        private AService Service;

        protected readonly Dictionary<long, Session> sessionDic = new Dictionary<long, Session>();
        private AppType appType;
        public AppType AppType { get { return appType; } }
        public virtual bool IsOuter { get { return false; } }

        public void Awake(NetworkProtocol protocol, AppType appType)
        {
            switch (protocol)
            {
                case NetworkProtocol.TCP:
                    this.Service = new TService();
                    break;
                case NetworkProtocol.UDP:
                    //this.Service = new UService();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Awake(NetworkProtocol protocol, IPEndPoint ipEndPoint, AppType appType)
        {
            this.appType = appType;
            switch (protocol)
            {
                case NetworkProtocol.TCP:
                    this.Service = new TService(ipEndPoint);
                    break;
                case NetworkProtocol.UDP:
                    //this.Service = new UService(host, port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.Service.AcceptCallback += this.OnAccept;
            this.Start();
        }
        public void Start()
        {
            this.Service.Start();
        }
        public void OnAccept(AChannel channel)
        {
            Session session = ObjectPoolManager.Instance.Take<Session, long, NetworkComponent, AChannel>(IdGenerater.GenerateId(), this, channel);
            channel.ErrorCallback += (c, e) => { this.Remove(session); };
            AddSession(session);
        }
        public virtual void AddSession(Session session)
        {
            this.sessionDic.Add(session.Id, session);
        }
        public virtual void Remove(Session session)
        {
            if (this.sessionDic.Remove(session.Id))
                session.Dispose();
        }

        public Session Get(long id)
        {
            Session session;
            this.sessionDic.TryGetValue(id, out session);
            return session;
        }

        /// <summary>
        /// 创建一个新Session
        /// </summary>
        public Session Create(IPEndPoint ipEndPoint)
        {
            AChannel channel = this.Service.ConnectChannel(ipEndPoint);
            Session session = ObjectPoolManager.Instance.Take<Session, long, NetworkComponent, AChannel>(IdGenerater.GenerateId(), this, channel);
            channel.ErrorCallback += (c, e) => { this.Remove(session); };
            this.AddSession(session);
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
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (Session session in this.sessionDic.Values.ToArray())
            {
                session.Dispose();
            }

            this.Service.Dispose();
        }
    }
}

