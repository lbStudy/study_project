using Base;
using Data;
using System.Collections.Generic;

namespace HttpHandle
{
    [Http(HttpRequestType.FindPlayerInfo, AppType.ManagerServer)]
    public class HttpFindPlayerInfo : IHttpHandle
    {
        public async void Run(HttpPackage httpPackage)
        {
            List<PlayerDetailData> playerdatas = null;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ret"] = 0;
            if (httpPackage.reqContent.stype == "all")
            {
                playerdatas = await DBOperateComponent.Instance.FindPlayerDetailData();
            }
            else if(httpPackage.reqContent.stype == "some")
            {
                if(httpPackage.reqContent.dataList != null && httpPackage.reqContent.dataList.Count > 0)
                {
                    List<long> playerids = new List<long>();
                    for (int i = 0; i < httpPackage.reqContent.dataList.Count; i++)
                    {
                        playerids.Add((long)httpPackage.reqContent.dataList[i]);
                    }
                    Dictionary<int, List<long>> onlinePlayerDic = new Dictionary<int, List<long>>();
                    List<long> offlinePlayers = new List<long>();
                    for (int i = 0; i < playerids.Count; i++)
                    {
                        PlayerAllotInfo info = ServerAllotComponent.Instance.Find(playerids[i]);
                        if (info != null)
                        {
                            List<long> onlinePlayers = null;
                            if (!onlinePlayerDic.TryGetValue(info.gateid, out onlinePlayers))
                            {
                                onlinePlayers = new List<long>();
                                onlinePlayerDic.Add(info.gateid, onlinePlayers);
                            }
                            onlinePlayers.Add(playerids[i]);
                        }
                        else
                        {
                            offlinePlayers.Add(playerids[i]);
                        }
                    }
                    int ret1 = 0;
                    if (onlinePlayerDic.Count > 0)
                    {
                        foreach (KeyValuePair<int, List<long>> pair in onlinePlayerDic)
                        {
                            Session gateSession = NetInnerComponent.Instance.GetByAppID(pair.Key);
                            M2Gate_GmFindPlayerInfoRequest reqToGate = new M2Gate_GmFindPlayerInfoRequest();
                            reqToGate.playerids = pair.Value;
                            Gate2M_GmFindPlayerInfoResponse respFromGate = await gateSession.Call(reqToGate) as Gate2M_GmFindPlayerInfoResponse;
                            if(respFromGate.errorCode != (int)ErrorCode.Success)
                            {
                                ret1 = 6;
                            }
                            else
                            {
                                if (respFromGate.playerdatas != null && respFromGate.playerdatas.Count > 0)
                                {
                                    playerdatas.AddRange(respFromGate.playerdatas);
                                }
                            }
                        }
                    }
                    int ret2 = 0;
                    if (offlinePlayers.Count > 0)
                    {
                        playerdatas = await DBOperateComponent.Instance.FindPlayerDetailData(offlinePlayers);
                    }
                    if(ret1 == 6 && ret2 == 6)
                    {
                        dic["ret"] = 6;
                    }
                }
            }
            else
            {
                dic["ret"] = 5;
            }
            dic["content"] = playerdatas;
            httpPackage.Response(dic);
        }
    }
}