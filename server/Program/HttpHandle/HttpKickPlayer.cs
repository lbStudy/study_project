using Base;
using Data;
using System.Collections.Generic;

namespace HttpHandle
{
    [Http(HttpRequestType.KickPlayer, AppType.ManagerServer)]
    public class HttpKickPlayer : IHttpHandle
    {
        public async void Run(HttpPackage httpPackage)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ret"] = 0;
            if(httpPackage.reqContent.dataList == null || httpPackage.reqContent.dataList.Count == 0)
            {
                dic["ret"] = 7;
            }
            else
            {
                Dictionary<int, List<long>> playeridDic = new Dictionary<int, List<long>>();
                List<long> playerids = null;
                foreach (double v in httpPackage.reqContent.dataList)
                {
                    long playerid = (long)v;
                    PlayerAllotInfo ai = ServerAllotComponent.Instance.Find(playerid);
                    if (ai == null)
                        continue;
                    if (!playeridDic.TryGetValue(ai.gateid, out playerids))
                    {
                        playerids = new List<long>();
                        playeridDic.Add(ai.gateid, playerids);
                    }
                    playerids.Add(playerid);
                }
                foreach (KeyValuePair<int, List<long>> pair in playeridDic)
                {
                    M2Gate_KickPlayerRequest reqToGate = new M2Gate_KickPlayerRequest();
                    reqToGate.playerids = pair.Value;
                    Session gateSession = NetInnerComponent.Instance.GetByAppID(pair.Key);
                    if (gateSession != null)
                    {
                        Gate2M_KickPlayerResponse respFromGate = await gateSession.Call(reqToGate) as Gate2M_KickPlayerResponse;
                    }
                }
            }
            
            httpPackage.Response(dic);
        }
    }
}
