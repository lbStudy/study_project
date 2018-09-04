using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using System.IO;
using Data;
using System.Net;

public class InnerNetInfo
{
    public int appId;
    public AppType appType;
    public int system;
    public Session session;
    public string innerIp;
    public int innerPort;
    public string outerIp;
    public int outerPort;
    public int allotCount;
}

public class NetInnerComponent : NetworkComponent, IAwake<AppType>, IAwake<IPEndPoint, AppType>
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

    public void Awake(IPEndPoint ipEndPoint, AppType appType)
    {
        Instance = this;
        this.Awake(NetworkProtocol.TCP, ipEndPoint, appType);

        ServerConfig registerConfig = null;
        if (AppType == AppType.ManagerServer)
        {
            registerConfig = ServerConfigComponent.Instance.loginServerConfig;
        }
        else if(appType != AppType.LoginServer)
        {
            registerConfig = ServerConfigComponent.Instance.managerServerConfig;
        }
        if(registerConfig != null)
            InnerRegister(registerConfig.innerip, registerConfig.innerport, registerConfig.appid, registerConfig.appType, registerConfig.system);
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
        appSessionDic.Clear();
    }
    public override void Remove(Session session)
    {
        base.Remove(session);
        foreach(InnerNetInfo netInfo in appSessionDic.Values)
        {
            if(netInfo.session == session)
            {
                netInfo.session = null;
                Console.WriteLine($"inner disconnect with {netInfo.appId}.");
                break;
            }
        }
    }
    public void Add(InnerNetInfo netInfo)
    {
        if (netInfo == null || netInfo.appId <= 0 || appSessionDic.ContainsKey(netInfo.appId))
            return;
        appSessionDic[netInfo.appId] = netInfo;
        List<InnerNetInfo> netInfos = null;
        if(appSessions.TryGetValue(netInfo.appType, out netInfos) == false)
        {
            netInfos = new List<InnerNetInfo>();
            appSessions.Add(netInfo.appType, netInfos);
        }
        netInfos.Add(netInfo);
    }
    public void RemoveByAppId(int serverId)
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
        if(appSessionDic.Remove(netInfo.appId))
        {
            List<InnerNetInfo> netInfos = null;
            if (appSessions.TryGetValue(netInfo.appType, out netInfos))
            {
                netInfos.Remove(netInfo);
            }
        }
    }
    public InnerNetInfo FindByAppId(int serverId)
    {
        InnerNetInfo netInfo = null;
        appSessionDic.TryGetValue(serverId, out netInfo);
        return netInfo;
    }
    public Session FindSessionByAppId(int serverId)
    {
        InnerNetInfo netInfo = null;
        if(appSessionDic.TryGetValue(serverId, out netInfo))
        {
            return netInfo.session;
        }
        return null;
    }
    public List<InnerNetInfo> FindByAppType(AppType appType)
    {
        List<InnerNetInfo> netInfos = null;
        appSessions.TryGetValue(appType, out netInfos);
        return netInfos;
    }
    public InnerNetInfo FindSystemServer(SystemType systemType)
    {
        int stype = (int)systemType;
        List<InnerNetInfo> netInfos = null;
        if(appSessions.TryGetValue(AppType.SystemServer, out netInfos))
        {
            foreach(InnerNetInfo netInfo in netInfos)
            {
                if((netInfo.system & stype) > 0)
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
    public void InnerRegister(string ip, int port, int appId, AppType appType, int system)
    {
        InnerNetInfo netInfo = FindByAppId(appId);
        if(netInfo != null)
        {
            if (netInfo.session != null && netInfo.session.IsDisposed == false)
                netInfo.session.Dispose();
            Remove(netInfo);
        }
        else
        {
            netInfo = new InnerNetInfo();
        }
        netInfo.appType = appType;
        netInfo.appId = appId;
        netInfo.system = system;
        netInfo.innerIp = ip;
        netInfo.innerPort = port;
        Add(netInfo);
        Console.WriteLine($"start inner connect {appId}.");
        netInfo.session = this.Create(NetworkHelper.ToIPEndPoint(ip, port), (bool isSuccess) =>
        {
            if (isSuccess)
            {
                Console.WriteLine($"inner connect success {appId}.");
                A2A_ServerRegisterMessage registerMsg = ProtocolDispatcher.Instance.Take<A2A_ServerRegisterMessage>((int)ProtoEnum.A2A_ServerRegisterMessage);
                try
                {
                    ServerConfig serverConfig = ServerConfigComponent.Instance.curServerConfig;
                    registerMsg.appId = serverConfig.appid;
                    registerMsg.appType = (int)serverConfig.appType;
                    registerMsg.innerIp = serverConfig.innerip;
                    registerMsg.innerPort = serverConfig.innerport;
                    registerMsg.outerIp = serverConfig.outerip;
                    registerMsg.outerPort = serverConfig.outerport;
                    registerMsg.system = serverConfig.system;
                    registerMsg.areaId = ServerConfigComponent.Instance.AreaId;
                    registerMsg.areaName = ServerConfigComponent.Instance.AreaName;
                    netInfo.session.relevanceID = appId;
                    netInfo.session.SendMessage(registerMsg, NetHelper.GetSendId(null));
                }
                finally
                {
                    ProtocolDispatcher.Instance.Back(registerMsg);
                }
                Game.Instance.SetInitFinishModule(InitModule.InnerConnect);
            }
            else
            {
                Console.WriteLine($"inner connect fail {appId}.");
                //InnerRegister(ipEndPoint, appId, appType, system);
            }
        });
    }
}