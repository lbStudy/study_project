using Base;
using Data;
using System;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(13563)]
    public class C2Gate_RequestOrderRequestHandler : AMRpcHandler<C2Gate_RequestOrderRequest>
    {
        protected override   async void Run(RpcPacakage pacakage)
        {
            C2Gate_RequestOrderRequest req = pacakage.msg as C2Gate_RequestOrderRequest;
            Gate2C_RequestOrderResponse response = pacakage.Response as Gate2C_RequestOrderResponse;

		    try
		    {
                    Player player = PlayerManagerComponent.Instance.Find(pacakage.Toid);
                    if(null != player)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>() { { "dataType", "payfor" }, { "userId",player.Id.ToString() }, { "sType", req.SType.ToString() }, { "gType", "5" }, { "gGet", req.Gem.ToString() }, { "serverId", ServerConfigComponent.Instance.Projectid.ToString() }, { "ext", req.ext } };

                        string content = await ClientHttpComponent.Instance.HttpPostAsync(ServerConfigComponent.Instance.GetWebUrl("pay"), MiniJSON.Json.Serialize(dic));
                       System.Object o =  MiniJSON.Json.Deserialize(content);
                        Dictionary<string, System.Object> os = o as Dictionary<string, System.Object>;
                        int retcode = System.Convert.ToInt32(os["retCode"].ToString());
                        if(retcode == 0)
                        {
                            response.serial = os["serial"].ToString();
                        }
                        else
                        {
                            response.error = (int)ErrorCode.Fail;
                        }
                    }
                    else
                    {
                        response.error = (int)ErrorCode.NotExistPlayer;
                    }

                }
		    catch (Exception e)
		    {
			    Console.WriteLine(e.ToString());
			    response.error = (int)ErrorCode.Fail;
		    }
		    finally
		    {
			    pacakage.Reply();
		    }
        }
    }
}
