using Base;
using Data;
using System;
using System.Threading.Tasks;

namespace ProtocolHandle
{
    [Protocol(32508)]
    public class C2L_EnterAreaRequestHandler : AMRpcHandler<C2L_EnterAreaRequest>
    {
        protected override async void Run(RpcPackage package)
        {
            C2L_EnterAreaRequest req = package.msg as C2L_EnterAreaRequest;
            L2C_EnterAreaResponse response = package.Response as L2C_EnterAreaResponse;

            try
            {
                if (package.Toid <= 0)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                LoginInfo info = LoginManagerComponent.Instance.FindLoginInfoById(package.Toid);
                if(info == null || info.session != package.Source)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                if(info.areaid != 0 && info.areaid != req.areaid)
                {//说明该玩家在其他区登录了,通知该区服务器移除玩家(目前不存在多区情况暂时不处理)
                    
                }
                AreaInfo areaInfo = LoginManagerComponent.Instance.FindAreaInfo(req.areaid);
                if(areaInfo == null || !areaInfo.isOpen)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                //BigAreaConfig bigAreaCf = ServerConfigComponent.Instance.GetBigAreaCfById(req.areaid);
                //Session managerSession = NetInnerComponent.Instance.GetByAppID(bigAreaCf.managerAppId);
                //L2M_EnterAreaRequest reqToM = new L2M_EnterAreaRequest();
                //reqToM.id = package.Toid;
                //M2L_EnterAreaResponse respFromM = await managerSession.Call(reqToM) as M2L_EnterAreaResponse;
                //if(respFromM.errorCode == (int)ErrorCode.Success)
                //{
                //    response.gateip = respFromM.gateip;
                //    response.gateport = respFromM.gateport;
                //    response.chatip = respFromM.chatip;
                //    response.chatport = respFromM.chatport;
                //    response.checkcode = respFromM.checkcode;
                //    info.state = LoginState.Entering;
                //}
                //else
                //{
                //    response.errorCode = respFromM.errorCode;
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
