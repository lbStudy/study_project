using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using System.IO;

public class InnerNetInfo
{
    public int serverId;
    public AppType appType;
    public int systemType;
    public int state;   // 1==正常 0==断开
    public Session session;
}

public class NetInnerComponent : NetworkComponent, IAwake<AppType>, IAwake<string, int, AppType>
{
    //private readonly Dictionary<int, Session> appSessions = new Dictionary<int, Session>();
    public static NetInnerComponent Instance;

    Dictionary<int, InnerNetInfo> appSessionDic = new Dictionary<int, InnerNetInfo>();
    Dictionary<AppType, List<InnerNetInfo>> appSessions = new Dictionary<AppType, List<InnerNetInfo>>();

    public void Awake(AppType appType)
    {
        Instance = this;
        this.Awake(NetworkProtocol.TCP, appType);
    }

    public void Awake(string host, int port, AppType appType)
    {
        Instance = this;
        this.Awake(NetworkProtocol.TCP, NetworkHelper.ToIPEndPoint(host, port), appType);
        //InnerConnect();
        //ServerPingSend();
        //ServerPingDetection();
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
        appSessions.Clear();
    }
    public void AddByServerId(InnerNetInfo netInfo)
    {
        if (netInfo == null || netInfo.serverId <= 0 || appSessionDic.ContainsKey(netInfo.serverId))
            return;
        appSessionDic[netInfo.serverId] = netInfo;
        List<InnerNetInfo> netInfos = null;
        if(appSessions.TryGetValue(netInfo.appType, out netInfos) == false)
        {
            netInfos = new List<InnerNetInfo>();
            appSessions.Add(netInfo.appType, netInfos);
        }
        netInfos.Add(netInfo);
    }
    public void RemoveByServerId(int serverId)
    {
        InnerNetInfo netInfo = null;
        if(appSessionDic.TryGetValue(serverId, out netInfo))
        {
            Remove(netInfo);
        }
    }
    public void Remove(InnerNetInfo netInfo)
    {
        if (netInfo == null)
            return;
        if(appSessionDic.Remove(netInfo.serverId))
        {
            List<InnerNetInfo> netInfos = null;
            if (appSessions.TryGetValue(netInfo.appType, out netInfos))
            {
                netInfos.Remove(netInfo);
            }
        }
    }
    public InnerNetInfo FindByServerId(int serverId)
    {
        InnerNetInfo netInfo = null;
        appSessionDic.TryGetValue(serverId, out netInfo);
        return netInfo;
    }
    public Session FindSessionByServerId(int serverId)
    {
        InnerNetInfo netInfo = null;
        if(appSessionDic.TryGetValue(serverId, out netInfo))
        {
            return netInfo.session;
        }
        return null;
    }
    public InnerNetInfo FindSystemServer(SystemType systemType)
    {
        int stype = (int)systemType;
        List<InnerNetInfo> netInfos = null;
        if(appSessions.TryGetValue(AppType.SystemServer, out netInfos))
        {
            foreach(InnerNetInfo netInfo in netInfos)
            {
                if((netInfo.systemType & stype) > 0)
                {
                    return netInfo;
                }
            }
        }
        return null;
    }
    //public async void ServerPingSend()
    //{
    //    while(true)
    //    {
    //        await Task.Delay(ConstConfigComponent.ConstConfig.ServerSendPing);
    //        if(Game.Instance.State == GameState.Runing)
    //        {
    //            A2A_ServerPingMessage msgToA = new A2A_ServerPingMessage();
    //            msgToA.fromApp = Game.Instance.Appid;
    //            List<Session> sessions = new List<Session>(appSessions.Values);
    //            for (int i = 0; i < sessions.Count; i++)
    //            {
    //                if(sessions[i].relevanceID > 0)
    //                    sessions[i].SendMessage(msgToA, 0);
    //            }
    //        }
    //    }
    //}
    //public async void ServerPingDetection()
    //{
    //    while (true)
    //    {
    //        await Task.Delay(ConstConfigComponent.ConstConfig.ServerPingTime);
    //        if (Game.Instance.State == GameState.Runing)
    //        {
    //            List<Session> sessions = new List<Session>(sessionDic.Values);
    //            for (int i = 0; i < sessions.Count; i++)
    //            {
    //                if (sessions[i].relevanceID == 0 && !sessions[i].IsDisposed && Game.Instance.Msec - sessions[i].pingTime > ConstConfigComponent.ConstConfig.ServerPingTime)
    //                {
    //                    //Console.WriteLine($"NetInner Disconnect {sessions[i].RemoteAddress} ");
    //                    Remove(sessions[i]);
    //                }
    //            }
    //        }
    //    }
    //}
    //async void InnerConnect()
    //{
    //    while(true)
    //    {
    //        bool isFinishInnerConnect = true;
    //        List<AppType> connectApps = ConstDefine.innerConnectDic[Game.Instance.AppType];
    //        for (int i = 0; i < connectApps.Count; i++)
    //        {
    //            AppType connectApp = connectApps[i];
    //            if(connectApp == Game.Instance.AppType)
    //            {
    //                Console.WriteLine($"error : exist same apptype({Game.Instance.AppType}) connect in ConstDefine.innerConnectDic");
    //                continue;
    //            }

    //            List<ServerConfig> connectServers = ServerConfigComponent.Instance.GetServerConfigByAppType(connectApp);
    //            if(connectServers != null && connectServers.Count > 0)
    //            {
    //                for(int j = 0; j < connectServers.Count; j++)
    //                {
    //                    ServerConfig connectConfig = connectServers[j];
    //                    if (appSessions.ContainsKey(connectConfig.appid) && !appSessions[connectConfig.appid].IsDisposed)
    //                    {
    //                        Console.WriteLine($"connect success : ({connectConfig.appid}){connectConfig.innerAddress}");
    //                        continue;
    //                    }
    //                    Session session = this.Create(NetworkHelper.ToIPEndPoint(connectConfig.innerip, connectConfig.innerport));
    //                    Console.WriteLine($"start connect : ({connectConfig.appid }){connectConfig.innerAddress}");
    //                    session.relevanceID = connectConfig.appid;
    //                    session.pingTime = Game.Instance.Msec;
    //                    this.appSessions.Add(connectConfig.appid, session);

    //                    isFinishInnerConnect = false;
    //                }
    //            }
    //        }
    //        if (isFinishInnerConnect)
    //        {
    //            Game.Instance.SetInitFinishModule(InitModule.InnerConnect);
    //            return;
    //        }                
    //        await Task.Delay(3000);
    //    }
    //}
    //public override void Remove(Session session)
    //{
    //    base.Remove(session);

    //    if (session.relevanceID > 0 && appSessions.Remove((int)session.relevanceID))
    //    {
    //        Console.WriteLine($"NetInner Disconnect {session.relevanceID} {session.RemoteAddress.ToString()} ");
    //        if(Game.Instance.State == GameState.Runing)
    //        {
    //            ServerConfig serverCf = ServerConfigComponent.Instance.GetServerConfigByAppid((int)session.relevanceID);
    //            InnectReconnect(serverCf);
    //        }
    //    }
    //}
    //public async void InnectReconnect(ServerConfig connectConfig)
    //{
    //    Session session = this.Create(NetworkHelper.ToIPEndPoint(connectConfig.innerip, connectConfig.innerport));
    //    Console.WriteLine($"start reconnect : ({connectConfig.appid }){connectConfig.innerAddress}");
    //    session.relevanceID = connectConfig.appid;
    //    session.pingTime = Game.Instance.Msec;
    //    this.appSessions.Add(connectConfig.appid, session);

    //    await Task.Delay(3000);

    //    if (appSessions.ContainsKey(connectConfig.appid) && !appSessions[connectConfig.appid].IsDisposed)
    //    {
    //        Console.WriteLine($"reconnect success : ({connectConfig.appid}){connectConfig.innerAddress}");
    //    }
    //}
    //public Session GetByAppID(int appid)
    //{
    //    Session session = null;
    //    appSessions.TryGetValue(appid, out session);
    //    return session;
    //}
    //public void SendMsgToAllServer(object msg)
    //{
    //    Packet packet = Packet.Take();
    //    try
    //    {
    //        Session.FillContent(packet.Stream, msg, 0, Game.Instance.Appid);
    //        foreach (Session session in appSessions.Values)
    //        {
    //            session.SendMessage(packet.Stream);
    //        }
    //    }
    //    catch(Exception e)
    //    {
    //        Log.Error(e.ToString());
    //    }
    //    finally
    //    {
    //        Packet.Back(packet);
    //    }
    //}
    //public void SendMsgToSevers(object msg, AppType appType)
    //{
    //    List<ServerConfig> cfs = ServerConfigComponent.Instance.GetServerConfigByAppType(appType);
    //    if (cfs == null || cfs.Count == 0)
    //        return;
    //    Packet packet = Packet.Take();
    //    try
    //    {
    //        Session.FillContent(packet.Stream, msg, 0, Game.Instance.Appid);
    //        for (int i = 0; i < cfs.Count; i++)
    //        {
    //            Session session = GetByAppID(cfs[i].appid);
    //            if (session != null)
    //                session.SendMessage(packet.Stream);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Log.Error(e.ToString());
    //    }
    //    finally
    //    {
    //        Packet.Back(packet);
    //    }
    //}
}