using Base;
using Data;
using System.Collections.Generic;

namespace HttpHandle
{
    [Http(HttpRequestType.GlobalNotice, AppType.ManagerServer)]
    public class HttpGlobalNoticePush : IHttpHandle
    {
        public void Run(HttpPackage httpPackage)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ret"] = 0;
            if (string.IsNullOrEmpty(httpPackage.reqContent.content))
            {
                dic["ret"] = 7;
            }
            else
            {
                M2Gate_GlobalNoticeMessage msgToGate = new M2Gate_GlobalNoticeMessage();
                msgToGate.content = httpPackage.reqContent.content;
                NetInnerComponent.Instance.SendMsgToSevers(msgToGate, AppType.GateServer);
            }
            httpPackage.Response(dic);
        }
    }
}
