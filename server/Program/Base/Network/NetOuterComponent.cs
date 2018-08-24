using Base;
using System.Collections.Generic;

public class NetOuterComponent : NetworkComponent, IAwake<AppType>, IAwake<string, int, AppType>
{
    public static NetOuterComponent Instance;
    Dictionary<long, Session> relevanceSessionDic = new Dictionary<long, Session>();


    public void Awake(AppType appType)
    {
        Instance = this;
        this.Awake(NetworkProtocol.TCP, appType);
    }

    public void Awake(string host, int port, AppType appType)
    {
        Instance = this;
        this.Awake(NetworkProtocol.TCP, NetworkHelper.ToIPEndPoint(host, port), appType);
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
        relevanceSessionDic.Clear();
    }
    private new void Update()
    {
        base.Update();
    }
    public void AddSessionByRelevanceID(Session session)
    {
        if(session.relevanceID > 0)
            relevanceSessionDic[session.relevanceID] = session;
    }
    public Session FindByRelevanceID(long relevanceID)
    {
        Session session = null;
        relevanceSessionDic.TryGetValue(relevanceID, out session);
        return session;
    }
    public override void Remove(Session session)
    {
        base.Remove(session);
        if (session.relevanceID > 0 && relevanceSessionDic.Remove(session.relevanceID))
        {
            EventDispatcher.Instance.Run((int)InnerEventIdType.OuterSessionDisconnect, session);
        }
    }
}
