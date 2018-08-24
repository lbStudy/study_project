using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(83560)]
    public class Gate2M_PlayerEnterGameRequestHandler : AMRpcHandler<Gate2M_PlayerEnterGameRequest>
    {
        protected override async void Run(RpcPackage package)
        {
            Gate2M_PlayerEnterGameRequest req = package.msg as Gate2M_PlayerEnterGameRequest;
            M2Gate_PlayerEnterGameResponse response = package.Response as M2Gate_PlayerEnterGameResponse;

            try
            {
                M2L_PlayerEnterGameRequest reqToL = new M2L_PlayerEnterGameRequest();
                reqToL.id = req.id;
                reqToL.bigAreaid = Game.Instance.BigAreaId;
                //Session loginSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.LoginAppId);
                //L2M_PlayerEnterGameResponse respFromL = await loginSession.Call(reqToL) as L2M_PlayerEnterGameResponse;
                //if(respFromL.errorCode == (int)ErrorCode.Success)
                //{
                //    ServerAllotComponent.Instance.PlayerEnter(req.id, req.gateAppid);
                //    response.name = respFromL.name;
                //    response.iconUrl = respFromL.iconUrl;
                //    response.sex = respFromL.sex;
                //}
                //else
                //{
                //    response.errorCode = respFromL.errorCode;
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
