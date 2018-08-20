using System;

namespace Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HttpAttribute : Attribute
    {
        public int requestType;
        public AppType appType;
        public HttpAttribute(HttpRequestType requestType, AppType appType)
        {
            this.requestType = (int)requestType;
            this.appType = appType;
        }
    }
}
