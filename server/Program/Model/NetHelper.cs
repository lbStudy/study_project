using Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

//serverId < 100w  map < 200w  charId > 10000w
public static class NetHelper
{
    public static Session GetSession(ProtocolInfo protoInfo, Player player)
    {
        if (protoInfo.ToServer == AppType.All)
        {
            return null;
        }
        Session session = null;

        switch(protoInfo.ToServer)
        {
            case AppType.LoginServer:
                if (NetInnerComponent.Instance.AppType == AppType.ManagerServer)
                {
                    List<InnerNetInfo> netInfos = NetInnerComponent.Instance.FindByAppType(AppType.LoginServer);
                    if(netInfos != null && netInfos.Count > 0)
                    {
                        session = netInfos[0].session;
                    }
                }
                else
                {
                    session = NetInnerComponent.Instance.FindSessionByAppId(ServerConfigComponent.Instance.ManagerAppId);
                }
                break;
            case AppType.ManagerServer:
                session = NetInnerComponent.Instance.FindSessionByAppId(ServerConfigComponent.Instance.ManagerAppId);
                break;
            case AppType.Client:
                if(player != null)
                {
                    if (NetInnerComponent.Instance.AppType == AppType.GateServer)
                    {
                        session = NetOuterComponent.Instance.FindByRelevanceID(player.Id);
                    }
                    else
                    {
                        session = NetInnerComponent.Instance.FindSessionByAppId(player.GetServerId(AppType.GateServer));
                    }
                }
                break;
            case AppType.GameServer:
            case AppType.MapServer:
            case AppType.BattleServer:
                if (player != null)
                {
                    if (NetInnerComponent.Instance.AppType == AppType.ManagerServer || NetInnerComponent.Instance.AppType == AppType.GateServer)
                    {
                        session = NetInnerComponent.Instance.FindSessionByAppId(player.GetServerId(protoInfo.ToServer));
                    }
                    else
                    {
                        session = NetInnerComponent.Instance.FindSessionByAppId(ServerConfigComponent.Instance.ManagerAppId);
                    }
                }
                break;
            case AppType.SystemServer:
                if (NetInnerComponent.Instance.AppType == AppType.ManagerServer || NetInnerComponent.Instance.AppType == AppType.GateServer)
                {
                    InnerNetInfo systemInfo = NetInnerComponent.Instance.FindSystemServer(protoInfo.SysType);
                    if (systemInfo != null)
                    {
                        session = systemInfo.session;
                    }
                }
                else
                {
                    session = NetInnerComponent.Instance.FindSessionByAppId(ServerConfigComponent.Instance.ManagerAppId);
                }
                break;
            case AppType.GateServer:
                if (player != null)
                {
                    if (NetInnerComponent.Instance.AppType == AppType.LoginServer)
                    {
                        session = NetInnerComponent.Instance.FindSessionByAppId(ServerConfigComponent.Instance.ManagerAppId);
                    }
                    else
                    {
                        session = NetInnerComponent.Instance.FindSessionByAppId(player.GetServerId(AppType.GateServer));
                    }
                }
                break;
        }
        return session;
    }
    /// <summary>
    /// 如果player == null && isBroadcast == true，对所有玩家广播
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isBroadcast"></param>
    /// <returns></returns>
    public static long GetSendId(Player player, bool isBroadcast = false)
    {
        if (player == null)
        {
            if (isBroadcast)
                return 0;
            else
                return -Game.Instance.Appid;
        }
        return player.Id;
    }

    //使用条件
    //不能使用在目标 ToServer==All
    //如果player == null, 不能向 多开服务器直接发送
    public static void SendMessage(object msg, Player player)
    {
        Type type = msg.GetType();
        ProtocolInfo protoInfo = ProtocolDispatcher.Instance.GetProtocolInfo(type);
        if (protoInfo.ProtocolCategory != ProtocolCategory.Message)
        {
            return;
        }
        Session session = GetSession(protoInfo, player);
        if (session != null)
            session.SendMessage(msg, GetSendId(player));
    }
    public static Task<object> Call(object request, Player player, object response = null)
    {
        Type type = request.GetType();
        ProtocolInfo protoInfo = ProtocolDispatcher.Instance.GetProtocolInfo(type);
        if (protoInfo.ProtocolCategory != ProtocolCategory.Request)
        {
            return null;
        }
        Session session = GetSession(protoInfo, player);
        if (session == null)
            return null;
        return session.Call(request, GetSendId(player), response);
    }
}