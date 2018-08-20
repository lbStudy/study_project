using Base;

public class NetOuterComponent : NetworkComponent, IAwake<AppType>, IAwake<string, int, AppType>
{
    public static NetOuterComponent Instance;

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
    }
    private new void Update()
    {
        base.Update();
    }
    public override void Remove(Session session)
    {
        base.Remove(session);
        EventDispatcher.Instance.Run((int)InnerEventIdType.OuterSessionDisconnect, session);
    }
}
