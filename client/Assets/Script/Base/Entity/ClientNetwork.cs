using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Base
{
    public enum SessionType
    {
        Login,
        Gate,
        Chat,
        Battle
    }
    public class ClientNetwork : Entity
    {
        static ClientNetwork instance;
        public static ClientNetwork Instance
        {
            get
            {
                if (instance == null)
                    instance = new ClientNetwork();
                return instance;
            }
        }

        public Dictionary<SessionType, Session> sessionDic = new Dictionary<SessionType, Session>();

        public ClientNetwork() : base(EntityType.None)
        {
            AddComponent<NetOuterComponent>();
        }
        public override void Dispose()
        {
            if (IsDisposer)
            {
                return;
            }
            base.Dispose();
            foreach(Session session in sessionDic.Values)
            {
                if(session.IsDisposer == false)
                    session.Dispose();
            }
            sessionDic.Clear();
            instance = null;
        }

        public bool IsConnect(SessionType sessionType)
        {
            Session session = null;
            sessionDic.TryGetValue(sessionType, out session);
            if (null == session)
                return false;
            if (IsDisposer)
                return false;
            return session.Connected;
        }
        public Task<bool> Connect(SessionType sessionType, string ip, int port)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Session session = NetOuterComponent.Instance.Create(ip, port, (string errormsg, System.Net.Sockets.SocketError errorcode) =>
            {
                tcs.SetResult(string.IsNullOrEmpty(errormsg));
            });
            sessionDic[sessionType] = session;
            return tcs.Task;
        }
        public Task<object> Call(object request, SessionType sessionType = SessionType.Gate)
        {
            Session session = null;
            sessionDic.TryGetValue(sessionType, out session);
            if (session == null)
                return null;
            if (session.IsDisposer)
            {
                Debug.Log("send session disconnect, but you use it");
                return null;
            }
            return session.Call(request);
        }
        public void Send(object message, SessionType sessionType = SessionType.Gate)
        {
            Session session = null;
            sessionDic.TryGetValue(sessionType, out session);
            if (session == null)
                return;
            if (session.IsDisposer)
            {
                Debug.Log("send session disconnect, but you use it");
                return;
            }
            session.Send(message);
        }

        public async void Send(object request, Action<object> response, SessionType sessionType = SessionType.Gate)
        {
            Session session = null;
            sessionDic.TryGetValue(sessionType, out session);
            if (session == null)
            {
                Debug.Log($"send session({sessionType.ToString()}) not exist, but you use it");
                return;
            }  
            if (session.IsDisposer)
            {
                Debug.Log($"send session({sessionType}) disconnect, but you use it");
                return;
            }
            object resp = await session.Call(request);
            if (response != null)
            {
                response(resp);
            }
        }
        public void Disconnect(SessionType sessionType)
        {
            Session session = null;
            sessionDic.TryGetValue(sessionType, out session);
            if (session == null)
                return;
            if (session.IsDisposer)
            {
                return;
            }
            session.Dispose();
        }
    }
}
