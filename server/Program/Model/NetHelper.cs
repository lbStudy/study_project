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
        if (player == null)
        {//向manager发送
            if (protoInfo.ToServer != AppType.ManagerServer && protoInfo.ToServer != AppType.SystemServer)
            {
                return null;
            }
            int serverId = ServerConfigComponent.Instance.ManagerAppId;
            session = NetInnerComponent.Instance.FindSessionByServerId(serverId);
        }
        else
        {
            if (protoInfo.ToServer == AppType.Client)
            {//发送客户端
                if (NetInnerComponent.Instance.AppType == AppType.GateServer)
                {
                    session = NetOuterComponent.Instance.FindByRelevanceID(player.Id);
                }
                else
                {
                    session = NetInnerComponent.Instance.FindSessionByServerId(player.GetServerId(AppType.GateServer));
                }
            }
            else
            {
                int serverId = 0;
                if (protoInfo.ToServer == AppType.GateServer)
                {
                    serverId = player.GetServerId(AppType.GateServer);
                }
                else if (protoInfo.ToServer == AppType.ManagerServer)
                {
                    serverId = ServerConfigComponent.Instance.ManagerAppId;
                }
                else
                {
                    if (NetInnerComponent.Instance.AppType == AppType.ManagerServer || NetInnerComponent.Instance.AppType == AppType.GateServer)
                    {
                        if (protoInfo.ToServer == AppType.SystemServer)
                        {
                            InnerNetInfo systemInfo = NetInnerComponent.Instance.FindSystemServer(protoInfo.SysType);
                            if (systemInfo == null)
                            {
                                serverId = 0;
                            }
                            else
                            {
                                serverId = systemInfo.serverId;
                            }
                        }
                        else
                        {
                            serverId = player.GetServerId(protoInfo.ToServer);
                        }
                    }
                    else
                    {
                        serverId = ServerConfigComponent.Instance.ManagerAppId;
                    }
                }
                session = NetInnerComponent.Instance.FindSessionByServerId(serverId);
            }
        }
        return session;
    }
    public static long GetSendId(Player player)
    {
        return player == null ? -Game.Instance.Appid : player.Id;
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