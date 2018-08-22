using Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    //serverId < 100w  map < 200w  charId > 10000w
    public static class NetHelper
    {
        public static void SendMessage(object msg, Player player)
        {
            Type type = msg.GetType();
            ProtocolInfo protoInfo = ProtocolDispatcher.Instance.GetProtocolInfo(type);
            if (protoInfo.ToServer == AppType.Client)
            {//发送客户端
                if (NetInnerComponent.Instance.AppType == AppType.GateServer)
                {
                    Session session = NetOuterComponent.Instance.Get(player.Id);
                    if (session == null)
                    {
                        //Console.WriteLine("Not")
                        return;
                    }
                    session.toids.Clear();
                    session.SendMessage(msg);
                }
                else
                {
                    InnerNetInfo netInfo = NetInnerComponent.Instance.Find(player.GetServerId(AppType.GateServer));
                    if (netInfo == null || netInfo.session == null)
                    {
                        return;
                    }
                    netInfo.session.toids.Clear();
                    netInfo.session.toids.Add(player.Id);
                    netInfo.session.SendMessage(msg);
                }
            }
            else
            {
                if(protoInfo.ToServer == AppType.All)
                {
                    return;
                }
                int serverId = 0;
                if (protoInfo.ToServer == AppType.GateServer)
                {
                    serverId = player.GetServerId(AppType.GateServer);
                }
                else
                {
                    if(NetInnerComponent.Instance.AppType == AppType.ManagerServer)
                    {
                        if(protoInfo.ToServer == AppType.SystemServer)
                        {
                            InnerNetInfo systemInfo = NetInnerComponent.Instance.FindSystemServer(protoInfo.SysTYpe);
                            if(systemInfo == null)
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
                InnerNetInfo netInfo = NetInnerComponent.Instance.Find(serverId);
                if (netInfo == null || netInfo.session == null)
                {
                    return;
                }
                netInfo.session.toids.Clear();
                netInfo.session.toids.Add(player.Id);
                netInfo.session.SendMessage(msg);
            }
        }
        public static void SendMessage(object msg, int serverId)
        {
            Type type = msg.GetType();
            ProtocolInfo protoInfo = ProtocolDispatcher.Instance.GetProtocolInfo(type);
            if (protoInfo.ToServer == AppType.Client)
            {
                return;
            }
            else
            {
                int toId = serverId;
                if (protoInfo.ToServer != AppType.GateServer)
                {
                    if (NetInnerComponent.Instance.AppType == AppType.ManagerServer)
                    {
                        if (protoInfo.ToServer == AppType.SystemServer)
                        {
                            InnerNetInfo systemInfo = NetInnerComponent.Instance.FindSystemServer(protoInfo.SysTYpe);
                            if (systemInfo == null)
                            {
                                serverId = 0;
                            }
                            else
                            {
                                serverId = systemInfo.serverId;
                            }
                        }
                    }
                    else
                    {
                        serverId = ServerConfigComponent.Instance.ManagerAppId;
                    }

                }
                InnerNetInfo netInfo = NetInnerComponent.Instance.Find(serverId);
                if (netInfo == null || netInfo.session == null)
                {
                    return;
                }
                netInfo.session.toids.Clear();
                netInfo.session.toids.Add(toId);
                netInfo.session.SendMessage(msg);
            }
        }
    }
}
