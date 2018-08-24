using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(60747)]
    public class L2M_EnterAreaRequestHandler : AMRpcHandler<L2M_EnterAreaRequest>
    {
        protected override async void Run(RpcPackage package)
        {
            L2M_EnterAreaRequest req = package.msg as L2M_EnterAreaRequest;
            M2L_EnterAreaResponse response = package.Response as M2L_EnterAreaResponse;
            try
            {
                //PlayerAllotInfo allotInfo = ServerAllotComponent.Instance.Find(req.id);
                //int gateid = 0;
                //if (allotInfo == null)
                //{
                //    gateid = ServerAllotComponent.Instance.GetGateApp();
                //}
                //else
                //{
                //    //if(allotInfo.state != PlayerState.Offline)
                //    //{
                //    //    response.errorCode = (int)ErrorCode.Fail;
                //    //    return;
                //    //}
                //    gateid = allotInfo.gateid;
                //}
                //Session gateSession = NetInnerComponent.Instance.GetByAppID(gateid);
                //M2Gate_PlayerConnectGateNotifyRequest reqToGate = new M2Gate_PlayerConnectGateNotifyRequest();
                //reqToGate.id = req.id;
                //Gate2M_PlayerConnectGateNotifyResponse respFromGate = await gateSession.Call(reqToGate) as Gate2M_PlayerConnectGateNotifyResponse;
                //if(respFromGate.errorCode == (int)ErrorCode.Success)
                //{
                //    ServerConfig gateServerCf = ServerConfigComponent.Instance.GetServerConfigByAppid(gateid);
                //    response.gateip = gateServerCf.outerip;
                //    response.gateport = gateServerCf.outerport;
                //    response.checkcode = respFromGate.checkcode;
                //}
                //else
                //{
                //    response.errorCode = respFromGate.errorCode;
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                package.Reply();
            }
        }
    }
}
