using Base;
using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtocolHandle
{
    [Protocol(26445)]
    public class C2L_ReloginRequestHandler : AMRpcHandler<C2L_ReloginRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            //int roleid = System.Convert.ToInt32(req.account);
            //L2C_ReloginResponse response = new L2C_ReloginResponse();
            //LoginInfo loginInfo = LoginManagerComponent.Instance.FindLoginInfoById(roleid);
            //if (loginInfo != null)
            //{
            //    if (loginInfo.state == LoginState.Login)
            //    {
            //        response.errorCode = (int)ErrorCode.Processing;
            //    }
            //    else
            //    {
            //        response.errorCode = (int)ErrorCode.Online;
            //    }
            //}
            //bool isNeedRemove = false;
            //try
            //{
            //    if (response.errorCode != (int)ErrorCode.Success)
            //    {
            //        return;
            //    }

            //    AreaInfo areaInfo = LoginManagerComponent.Instance.FindAreaInfo(req.areaid);
            //    if (areaInfo == null || !areaInfo.isOpen)
            //    {
            //        response.errorCode = (int)ErrorCode.Fail;
            //        return;
            //    }

            //    isNeedRemove = true;
            //    loginInfo = LoginManagerComponent.Instance.StartLogin(req.account);

            //    //Dictionary<string, string> dic = new Dictionary<string, string>() { { "action", "login" }, { "account", req.account }, { "psw", req.password } };
            //    //string content = await ClientHttpComponent.Instance.HttpPostAsync(ServerConfigComponent.Instance.GetWebUrl("login"), dic);
            //    //if (string.IsNullOrEmpty(content))
            //    //{
            //    //    response.errorCode = (int)ErrorCode.Fail;
            //    //}
            //    //else
            //    //{
            //    //    LoginReply login = (LoginReply)JsonConvert.DeserializeObject(content, typeof(LoginReply));
            //    //    if (login.ret == 0)
            //    //    {
            //    //        response.id = login.id;
            //    //        Log.Debug($"{login.id} login success .");
            //    //    }
            //    //    else
            //    //    {
            //    //        response.errorCode = (int)ErrorCode.Fail;
            //    //        Log.Debug($"{req.account} {req.password} login fail.");
            //    //    }
            //    //}
            //    response.id = roleid;
            //    if (response.errorCode == (int)ErrorCode.Success)
            //    {
            //        BigAreaConfig bigAreaCf = ServerConfigComponent.Instance.GetBigAreaCfById(req.areaid);
            //        Session managerSession = NetInnerComponent.Instance.GetByAppID(bigAreaCf.managerAppId);
            //        L2M_EnterAreaRequest reqToM = new L2M_EnterAreaRequest();
            //        reqToM.id = response.id;
            //        M2L_EnterAreaResponse respFromM = await managerSession.Call<L2M_EnterAreaRequest>(reqToM);
            //        if (respFromM.errorCode == (int)ErrorCode.Success)
            //        {
            //            response.gateip = respFromM.gateip;
            //            response.gateport = respFromM.gateport;
            //            response.checkcode = respFromM.checkcode;
            //            loginInfo.id = response.id;
            //            loginInfo.state = LoginState.Entering;
            //            loginInfo.password = req.password;
            //            loginInfo.areaid = req.areaid;
            //            LoginManagerComponent.Instance.FinishLogin(loginInfo);
            //            isNeedRemove = false;
            //        }
            //        else
            //        {
            //            response.errorCode = respFromM.errorCode;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    response.errorCode = (int)ErrorCode.CodeError;
            //    Log.Error(e.ToString());
            //}
            //finally
            //{
            //    if (loginInfo != null && isNeedRemove)
            //    {
            //        LoginManagerComponent.Instance.RemoveLoginInfo(System.Convert.ToInt32(req.account));
            //    }
            //    Session sourceSession = reply.Source;
            //    reply.Reply(response);
            //    await Task.Delay(2000);
            //    sourceSession.Dispose();
            //}
        }
    }
}
