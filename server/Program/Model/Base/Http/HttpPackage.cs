using System;
using System.Collections.Generic;
using System.Net;

namespace Base
{
    public class HttpPackage
    {
        public HttpRequestContent reqContent;
        public HttpListenerContext context;
        public bool isResponse;
        public void Init(HttpRequestContent reqContent, HttpListenerContext context)
        {
            this.reqContent = reqContent;
            this.context = context;
            isResponse = false;
        }
        public void Response(Dictionary<string, object> responseContent)
        {
            if (isResponse)
                return;
            try
            {
                string content = LitJson.JsonMapper.ToJson(responseContent);
                byte[] bys = System.Text.Encoding.UTF8.GetBytes(content);
                context.Response.ContentLength64 = bys.Length;
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                context.Response.ContentType = context.Request.ContentType;
                context.Response.OutputStream.Write(bys, 0, bys.Length);
            }
            catch
            {
                Log.Warning("Response error!");
            }
            finally
            {
                isResponse = true;
                context.Response.Close();
            }
        }
    }
}
