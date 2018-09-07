using LitJson;
using System;
using System.Collections.Generic;
using System.Net;

namespace Base
{
    public class HttpRequestContent
    {
        public string type;
        public string stype;
        public List<double> dataList;
        public string pid;
        public int amount;
        public string content;
        //public List<ActivityInfo> activitys;
    }
	/// <summary>
	/// http请求分发器
	/// </summary>
	public class HttpComponent : Component, IAwake<string, int>
    {
		public HttpListener listener;

		public void Awake(string ip, int port)
		{
            try
			{
				this.listener = new HttpListener();
				this.listener.Prefixes.Add($"http://{ip}:{port}/");
				this.listener.Start();
				this.Accept();
			}
			catch (HttpListenerException e)
			{
				throw new Exception($"http server error: {e.ErrorCode}", e);
			}
		}

		public async void Accept()
		{
            while (true)
			{
				if (this.IsDisposed)
				{
					return;
				}

				HttpListenerContext context = await this.listener.GetContextAsync();
                if(context.Request.HttpMethod == "POST")
                {
                    Log.Debug($"request from {context.Request.RemoteEndPoint.ToString()}");
                    Read(context);
                }
                else
                {

                }
            }
		}
        public async void Read(HttpListenerContext context)
        {
            byte[] bs = new byte[context.Request.ContentLength64];
            int lenght = 0;
            while(lenght < context.Request.ContentLength64)
            {
                int count = await context.Request.InputStream.ReadAsync(bs, lenght, (int)context.Request.ContentLength64 - lenght);
                lenght += count;
            }
            string content = System.Text.Encoding.UTF8.GetString(bs, 0, lenght);
            //Dictionary<string, object> ret = null;
            HttpRequestContent reqContent = null;
            try
            {
                reqContent = JsonMapper.ToObject<HttpRequestContent>(content);
                //ret = MiniJSON.Json.Deserialize(content) as Dictionary<string, object>;
                if(reqContent == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                Dictionary<string, object> responseContent = new Dictionary<string, object>() { { "ret", 1/*"deserialize_error"*/ } };
                content = MiniJSON.Json.Serialize(responseContent);
                byte[] bys = System.Text.Encoding.UTF8.GetBytes(content);
                context.Response.ContentLength64 = bys.Length;
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                context.Response.ContentType = context.Request.ContentType;
                context.Response.OutputStream.Write(bys, 0, bys.Length);
                context.Response.Close();
                Console.WriteLine($"http content deserialize error. content; {content}");
                Log.Warning($"http content deserialize error. content; {content}");
                return;
            }
            HttpPackage httpPackage = new HttpPackage();
            httpPackage.Init(reqContent, context);
            HttpDispatcher.Instance.Handle(httpPackage);
        }
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			this.listener.Stop();
			this.listener.Close();
		}
	}
}