using Base;
using System.Collections.Generic;
using System.Net;

public class NetOuterComponent : NetworkComponent, IAwake<AppType>, IAwake<IPEndPoint, AppType>
{
    public static NetOuterComponent Instance;
    Dictionary<long, Session> relevanceSessionDic = new Dictionary<long, Session>();

    public override bool IsOuter { get { return true; } }
    public void Awake(AppType appType)
    {
        Instance = this;
        this.Awake(NetworkProtocol.TCP, appType);
    }

    public void Awake(IPEndPoint ipEndPoint, AppType appType)
    {
        Instance = this;
        this.Awake(NetworkProtocol.TCP, ipEndPoint, appType);
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
