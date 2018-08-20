using Base;
using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(92142)]
    public class C2L_LoginRequestHandler : AMRpcHandler<C2L_LoginRequest>
    {
        void LoginNoSdk(C2L_LoginRequest req, RpcPacakage pacakage)
        {
            long roleId = System.Convert.ToInt64(req.account);
            Login(roleId, pacakage, null, roleId.ToString());
        }
        void Login(long roleId, RpcPacakage pacakage, string iconUrl = null, string name = null, int sex = 1)
        {
            L2C_LoginResponse response = pacakage.Response as L2C_LoginResponse;
            try
            {
                LoginInfo loginInfo = LoginManagerComponent.Instance.FindLoginInfoById(roleId);
                if(loginInfo == null)
                {
                    loginInfo = LoginManagerComponent.Instance.FinishLogin(roleId);
                    loginInfo.areaid = 0;
                    loginInfo.IsInGame = false;
                }
                loginInfo.state = LoginState.Logining;
                loginInfo.sex = sex;
                loginInfo.Name = name;
                loginInfo.iconUrl = iconUrl;
                loginInfo.session = pacakage.Source;
                response.areas = LoginManagerComponent.Instance.areas;
                response.id = roleId;
                pacakage.Source.relevanceID = roleId;
            }
            catch (Exception e)
            {
                response.errorCode = (int)ErrorCode.CodeError;
                Log.Error(e.ToString());
            }
            finally
            {
                pacakage.Reply();
            }
        }
       async void LoginSdk(C2L_LoginRequest req, RpcPacakage pacakage)
        {
         
            Dictionary<string, string> dic = new Dictionary<string, string>() { { "dataType", "auth" }, { "accountname", req.account }, { "password", req.password }, { "sType", req.channel.ToString() } , { "gType", "5" }, { "serverId", "0" } };
          
            string content = await ClientHttpComponent.Instance.HttpPostAsync(ServerConfigComponent.Instance.GetWebUrl("login"), MiniJSON.Json.Serialize(dic));
            if(!string.IsNullOrEmpty(content))
            {
                try
                {
                    Dictionary<string, System.Object> ret = MiniJSON.Json.Deserialize(content) as Dictionary<string, System.Object>;
                    if(null != ret)
                    {
                        int retcode = System.Convert.ToInt32(ret["retCode"].ToString());
                        if(retcode == 0)
                        {
                            int roleId  = System.Convert.ToInt32(ret["userId"].ToString());
                            Dictionary<string, System.Object> extenData = ret["extdata"] as Dictionary<string, System.Object>;
                            string nickName = extenData["nickname"].ToString();
                            string iconUrl = extenData["headimgurl"].ToString();
                            int sex = System.Convert.ToInt32(extenData["sex"].ToString());
                            Login(roleId, pacakage, iconUrl,nickName);
                        }
                        else
                        {
                            L2C_LoginResponse response = pacakage.Response as L2C_LoginResponse;
                            response.errorCode = retcode;
                            pacakage.Reply();
                        }
                    }
                    else
                    {
                        L2C_LoginResponse response = pacakage.Response as L2C_LoginResponse;
                        response.errorCode =(int) ErrorCode.Fail;
                        pacakage.Reply();
                    }
                }
                catch(System.Exception e)
                {
                    Console.WriteLine(e.ToString());
                    L2C_LoginResponse response = pacakage.Response as L2C_LoginResponse;
                    response.errorCode = (int)ErrorCode.Fail;
                    pacakage.Reply();
                }
            }

        }
        protected override  void Run(RpcPacakage pacakage)
        {
            C2L_LoginRequest req = pacakage.msg as C2L_LoginRequest;
            B2C_ExitRoomResponse response = pacakage.Response as B2C_ExitRoomResponse;

            if (req.channel == 0)
            {
                if(ConstConfigComponent.ConstConfig.IsNoSDKLogin <= 0)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    pacakage.Reply();
                    return;
                }
                LoginNoSdk(req, pacakage);
            }
            else
                LoginSdk(req, pacakage);
        }
    }
}
