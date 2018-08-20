using Base;
using Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(93721)]
    public class C2L_RegisterRequestHandler : AMRpcHandler<C2L_RegisterRequest>
    {
        protected override async void Run(RpcPacakage pacakage)
        {
            C2L_RegisterRequest req = pacakage.msg as C2L_RegisterRequest;
            L2C_RegisterResponse response = pacakage.Response as L2C_RegisterResponse;

            if(LoginManagerComponent.Instance.IsExistRegisterAccount(req.account))
            {
                response.errorCode = (int)ErrorCode.Processing;
            }
            bool isRemove = false;
            try
            {
                if(response.errorCode == (int)ErrorCode.Success)
                {
                    isRemove = true;
                    LoginManagerComponent.Instance.AddRegisterAccount(req.account);
                    Dictionary<string, string> dic = new Dictionary<string, string>() { { "action", "register" }, { "account", req.account }, { "psw", req.password } };
                    string content = await ClientHttpComponent.Instance.HttpPostAsync(ServerConfigComponent.Instance.GetWebUrl("register"), dic);
                    if (string.IsNullOrEmpty(content))
                    {
                        response.errorCode = (int)ErrorCode.Fail;
                    }
                    else
                    {
                        LoginReply login = (LoginReply)JsonConvert.DeserializeObject(content, typeof(LoginReply));
                        if (login.ret == 0)
                        {
                            Log.Debug($"{login.id} register success .");
                        }
                        else
                        {
                            response.errorCode = (int)ErrorCode.Fail;
                            Log.Debug($"{req.account} {req.password} register fail.");
                        }
                    }
                }
            }
            catch
            {
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                if(isRemove)
                {
                    LoginManagerComponent.Instance.RemoveRegisterAccount(req.account);
                }
                pacakage.Reply();
            }
        }
    }
}
