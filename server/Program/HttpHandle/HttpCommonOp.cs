using Base;
using Data;
using System.Collections.Generic;

namespace HttpHandle
{
    [Http(HttpRequestType.CommonOp, AppType.ManagerServer)]
    public class HttpCommonOp : IHttpHandle
    {
        public async void Run(HttpPackage httpPackage)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ret"] = 0;
            if (httpPackage.reqContent.stype == "addroomcard")
            {
                if(httpPackage.reqContent.amount == 0)
                {
                    dic["ret"] = 7;
                }
                else
                {
                    if (httpPackage.reqContent.dataList != null && httpPackage.reqContent.dataList.Count > 0)
                    {
                        long playerid = (long)httpPackage.reqContent.dataList[0];
                        PlayerAllotInfo info = ServerAllotComponent.Instance.Find(playerid);
                        if (info != null)
                        {
                            M2Gate_GmCommonOpRequest reqToGate = new M2Gate_GmCommonOpRequest();
                            reqToGate.id = playerid;
                            reqToGate.op = 1;//添加
                            reqToGate.param1 = 1;//房卡
                            reqToGate.param2 = httpPackage.reqContent.amount * 100;//数量
                            Session gateSession = NetInnerComponent.Instance.GetByAppID(info.gateid);
                            Gate2M_GmCommonOpResponse respFromGate = await gateSession.Call(reqToGate) as Gate2M_GmCommonOpResponse;
                            if (respFromGate.errorCode != (int)ErrorCode.Success)
                            {
                                dic["ret"] = 6;
                            }
                        }
                        else
                        {
                            if (DBOperateComponent.Instance.IsConnect())
                            {
                                PlayerDetailData detail = await DBOperateComponent.Instance.FindDetaildata(playerid);
                                if (detail == null)
                                {
                                    dic["ret"] = 6;
                                    return;
                                }
                                int modifiedCount = await DBOperateComponent.Instance.AddRoomCardAsync(playerid, httpPackage.reqContent.amount * 100);
                                if (modifiedCount != 1)
                                {
                                    dic["ret"] = 6;
                                    return;
                                }
                                else
                                {
                                    Log.Info(LogDatabase.fengyuncard, ServerConfigComponent.Instance.Projectid, detail.playerid, 1, detail.name, LogType.roomcard.ToString(), LogAction.gm_fillcard.ToString(), detail.roomcard, httpPackage.reqContent.amount * 100);
                                }
                            }
                            else
                            {
                                dic["ret"] = 6;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                dic["ret"] = 5;
            }
            httpPackage.Response(dic);
        }
    }
}