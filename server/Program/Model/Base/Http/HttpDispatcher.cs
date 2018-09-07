using System;
using System.Collections.Generic;
using System.Reflection;

namespace Base
{
    public class HttpDispatcher
    {
        Dictionary<int, IHttpHandle> httpHandleDic = new Dictionary<int, IHttpHandle>();

        const string actionStr = "type";


        private static HttpDispatcher instance;
        public static HttpDispatcher Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new HttpDispatcher();
                }
                return instance;
            }
        }

        public void Load(Assembly assembly, AppType appType)
        {
            httpHandleDic.Clear();
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {//将消息处理与消息码关联
                object[] attrs = type.GetCustomAttributes(typeof(HttpAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }
                HttpAttribute messageAttribute = (HttpAttribute)attrs[0];

                if (!httpHandleDic.ContainsKey(messageAttribute.requestType))
                {
                    if(messageAttribute.appType == AppType.All || messageAttribute.appType == appType)
                    {
                        IHttpHandle obj = Activator.CreateInstance(type) as IHttpHandle;
                        httpHandleDic.Add(messageAttribute.requestType, obj);
                    }
                }
                else
                {
                    Log.Debug($"exist same http handle, requestType : {messageAttribute.requestType}");
                }
            }
        }
        public void Handle(HttpPackage httpPackage)
        {
            if(string.IsNullOrEmpty(httpPackage.reqContent.type))
            {
                Dictionary<string, object> dic = new Dictionary<string, object>() { { "ret", 2/*$"{httpPackage.reqContent.type}_handle_not_exception"*/ } };
                httpPackage.Response(dic);
                Log.Warning($"http request type is null.");
                return;
            }
            HttpRequestType requestType = EnumHelper.FromString<HttpRequestType>(httpPackage.reqContent.type);
            if(requestType == HttpRequestType.None)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>() { { "ret", 2/*$"{httpPackage.reqContent.type}_handle_not_exception"*/ } };
                httpPackage.Response(dic);
                Log.Warning($"http request type({httpPackage.reqContent.type}) not exist.");
                return;
            }
            IHttpHandle handle = null;
            if (httpHandleDic.TryGetValue((int)requestType, out handle))
            {
                try
                {
                    handle.Run(httpPackage);
                }
                catch
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>() { { "ret", 4/*$"{httpPackage.reqContent.type}_handle_not_exception"*/ } };
                    httpPackage.Response(dic);
                    Log.Warning($"http type({httpPackage.reqContent.type}) request handle exception.");
                }
            }
            else
            {
                Dictionary<string, object> dic = new Dictionary<string, object>() { { "ret", 3/*$"{httpPackage.reqContent.type}_handle_not_exist"*/ } };
                httpPackage.Response(dic);
                Log.Warning($"Not exist http type({httpPackage.reqContent.type}) handle function.");
            }
        }
    }
}
