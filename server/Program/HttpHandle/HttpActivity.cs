using Base;
using Data;
using System.Collections.Generic;

namespace HttpHandle
{
    [Http(HttpRequestType.Activity, AppType.ManagerServer)]
    public class HttpActivity : IHttpHandle
    {
        public void Run(HttpPackage httpPackage)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ret"] = 0;

            //FuncDispatcher.Instance.Run((int)FunctionId.RefreshActivity, httpPackage.reqContent.activitys, true);

            httpPackage.Response(dic);
        }
    }
}
