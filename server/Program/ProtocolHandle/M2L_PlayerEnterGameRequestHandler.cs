using Base;
using System;
using Data;

namespace ProtocolHandle
{
    [Protocol(69717)]
    public class M2L_PlayerEnterGameRequestHandler : AMRpcHandler<M2L_PlayerEnterGameRequest>
    {
        protected override void Run(RpcPackage package)
        {
            M2L_PlayerEnterGameRequest req = package.msg as M2L_PlayerEnterGameRequest;
            L2M_PlayerEnterGameResponse response = package.Response as L2M_PlayerEnterGameResponse;

            try
            {
                LoginInfo info = LoginManagerComponent.Instance.FindLoginInfoById(req.id);
                if(info == null)
                {
                    response.errorCode = (int)ErrorCode.NotExistPlayer;
                    return;
                }
                info.IsInGame = true;
                info.state = LoginState.None;
                info.areaid = req.bigAreaid;
                response.name = info.Name;
                response.iconUrl = info.iconUrl;
                response.sex = info.sex;
                if(info.session != null)
                {
                    info.session.relevanceID = 0;
                    info.session.Dispose();
                    info.session = null;
                }
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
