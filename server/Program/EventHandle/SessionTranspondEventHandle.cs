using Base;
using Data;
using System.Collections.Generic;
using System.IO;

namespace EventHandle
{
    [Event((int)Base.InnerEventIdType.SessionTranspond, AppType.GateServer)]
    public class GateTranspondEventHandle : IEvent<Session, ProtocolInfo, List<long>, MemoryStream>
    {
        public static byte[] countByte = new byte[2];
        public void Run(Session session, ProtocolInfo protocolInfo, List<long> toIds, MemoryStream stream)
        {
            if (protocolInfo.ToServer == AppType.Client)
            {//转发客户端

                if (toIds.Count == 1)
                {
                    long id = toIds[0];
                    if(id == 0)
                    {//对所有人广播
                        List<Entity> players = EntityManager.Instance.FindByType(EntityType.Player);
                        foreach(Entity e in players)
                        {
                            Session clientSession = NetOuterComponent.Instance.FindByRelevanceID(e.Id);
                            if (clientSession != null)
                                clientSession.SendMessage(stream);
                        }
                    }
                    else
                    {
                        Session clientSession = NetOuterComponent.Instance.FindByRelevanceID(id);
                        if (clientSession != null)
                            clientSession.SendMessage(stream);
                    }
                }
                else
                {
                    Packet packet = null;
                    try
                    {
                        int offset = 2 + toIds.Count * 8;
                        int length = (int)stream.Length - offset;
                        packet = Packet.Take(length + countByte.Length);
                        packet.Stream.Write(countByte, 0, countByte.Length);
                        packet.Stream.Write(stream.GetBuffer(), offset, length);
                        foreach (long id in toIds)
                        {
                            Session clientSession = NetOuterComponent.Instance.FindByRelevanceID(id);
                            if (clientSession != null)
                                clientSession.SendMessage(packet.Stream);
                        }
                    }
                    finally
                    {
                        if (packet != null)
                        {
                            Packet.Back(packet);
                        }
                    }
                }
            }
            else if (protocolInfo.FromServer == AppType.Client)
            {//来自客户端,转发其他服务器
                Player player = EntityManager.Instance.Find(toIds[0]) as Player;
                Session toSession = NetHelper.GetSession(protocolInfo, player);
                if(toSession != null)
                    toSession.SendMessage(stream);
            }
        }
    }

    [Event((int)Base.InnerEventIdType.SessionTranspond, AppType.ManagerServer)]
    public class ManagerTranspondEventHandle : IEvent<Session, ProtocolInfo, List<long>, MemoryStream>
    {
        public static byte[] countByte = new byte[8];
        Dictionary<long, List<long>> appDic = new Dictionary<long, List<long>>();
        
        public void Run(Session session, ProtocolInfo protocolInfo, List<long> toIds, MemoryStream stream)
        {
            switch(protocolInfo.ToServer)
            {
                case AppType.LoginServer:
                    {
                        List<InnerNetInfo> netInfos = NetInnerComponent.Instance.FindByAppType(AppType.LoginServer);
                        if (netInfos != null && netInfos.Count > 0)
                        {
                            netInfos[0].session.SendMessage(stream);
                        }
                    }

                    break;
                case AppType.Client:
                case AppType.GateServer:
                    {
                        if (toIds.Count > 1)
                        {
                            int serverId = 0;
                            List<long> playerIds = null;
                            List<long> serverIds = CacheList.Take();
                            appDic.Clear();
                            foreach (long id in toIds)
                            {
                                Player player = EntityManager.Instance.Find(id) as Player;
                                if (player == null)
                                    continue;
                                serverId = player.GetServerId(AppType.GateServer);
                                if (serverId == 0)
                                    continue;
                                if (appDic.TryGetValue(serverId, out playerIds) == false)
                                {
                                    playerIds = CacheList.Take();
                                    appDic.Add(serverId, playerIds);
                                    serverIds.Add(serverId);
                                }
                                playerIds.Add(id);
                            }
                            int offset = 2 + toIds.Count * 8;
                            int length = (int)stream.Length - offset;
                            foreach (long sId in serverIds)
                            {
                                session = NetInnerComponent.Instance.FindSessionByAppId((int)sId);
                                playerIds = appDic[sId];
                                if (session != null && session.IsDisposed == false)
                                {
                                    Packet packet = Packet.Take(length + 2 + playerIds.Count * 8);
                                    countByte.WriteTo(0, (short)playerIds.Count);
                                    packet.Stream.Write(countByte, 0, 2);
                                    foreach (long pId in playerIds)
                                    {
                                        countByte.WriteTo(0, pId);
                                        packet.Stream.Write(countByte, 0, countByte.Length);
                                    }
                                    packet.Stream.Write(stream.GetBuffer(), offset, length);
                                    session.SendMessage(packet.Stream);
                                    Packet.Back(packet);
                                }
                                CacheList.Back(playerIds);
                            }
                            CacheList.Back(serverIds);
                        }
                        else if (toIds.Count == 1)
                        {
                            long id = toIds[0];
                            if (id > 0)
                            {
                                Player player = EntityManager.Instance.Find(toIds[0]) as Player;
                                session = NetInnerComponent.Instance.FindSessionByAppId(player.GetServerId(AppType.GateServer));
                                session.SendMessage(stream);
                            }
                            else if (id == 0)
                            {//转发所有网关
                                List<InnerNetInfo> netInfos = NetInnerComponent.Instance.FindByAppType(AppType.GateServer);
                                foreach(InnerNetInfo netInfo in netInfos)
                                {
                                    netInfo.session.SendMessage(stream);
                                }
                            }
                            else if (id < 0)
                            {
                                session = NetInnerComponent.Instance.FindSessionByAppId((int)(-id));
                                if(session != null)
                                    session.SendMessage(stream);
                            }
                        }
                    }
                    break;
                case AppType.GameServer:
                case AppType.MapServer:
                case AppType.BattleServer:
                    {
                        if (toIds.Count > 1)
                        {//玩家id
                            int serverId = 0;
                            List<long> playerIds = null;
                            List<long> serverIds = CacheList.Take();
                            appDic.Clear();
                            foreach (long id in toIds)
                            {
                                Player player = EntityManager.Instance.Find(id) as Player;
                                if (player == null)
                                    continue;
                                serverId = player.GetServerId(protocolInfo.ToServer);
                                if (serverId == 0)
                                    continue;
                                if (appDic.TryGetValue(serverId, out playerIds) == false)
                                {
                                    playerIds = CacheList.Take();
                                    appDic.Add(serverId, playerIds);
                                    serverIds.Add(serverId);
                                }
                                playerIds.Add(id);
                            }
                            int offset = 2 + toIds.Count * 8;
                            int length = (int)stream.Length - offset;
                            foreach (long sId in serverIds)
                            {
                                session = NetInnerComponent.Instance.FindSessionByAppId((int)sId);
                                playerIds = appDic[sId];
                                if (session != null && session.IsDisposed == false)
                                {
                                    Packet packet = Packet.Take(length + 2 + playerIds.Count * 8);
                                    countByte.WriteTo(0, (short)playerIds.Count);
                                    packet.Stream.Write(countByte, 0, 2);
                                    foreach (long pId in playerIds)
                                    {
                                        countByte.WriteTo(0, pId);
                                        packet.Stream.Write(countByte, 0, countByte.Length);
                                    }
                                    packet.Stream.Write(stream.GetBuffer(), offset, length);
                                    session.SendMessage(packet.Stream);
                                    Packet.Back(packet);
                                }
                                CacheList.Back(playerIds);
                            }
                            CacheList.Back(serverIds);
                        }
                        else
                        {
                            long id = toIds[0];
                            if (id < 0)
                            {//appid
                                session = NetInnerComponent.Instance.FindSessionByAppId((int)(-id));
                            }
                            else
                            {//玩家id
                                Player player = EntityManager.Instance.Find(id) as Player;
                                session = NetInnerComponent.Instance.FindSessionByAppId(player.GetServerId(protocolInfo.ToServer));
                            }
                            session.SendMessage(stream);
                        }
                    }
                    break;
                case AppType.SystemServer:
                    {
                        InnerNetInfo systemInfo = NetInnerComponent.Instance.FindSystemServer(protocolInfo.SysType);
                        if (systemInfo != null)
                        {
                            systemInfo.session.SendMessage(stream);
                        }
                    }
                    break;
            }
        }
    }
}
