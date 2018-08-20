using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Base;

public class ClientHttpComponent : Component, IAwake
{
    public static ClientHttpComponent Instance;
    private static readonly string DefaultUserAgent = ".net test.";//"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
    public void Awake()
    {
        Instance = this;
        //Send();
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
    }
    public async void Send()
    {
        await Task.Delay(3000);
        Dictionary<string, string> dic = new Dictionary<string, string>() { { "dataType", "auth" }, { "accountname", "xxx" }, { "password", "123"}, { "sType", "123" }, { "gType", "5" }, { "serverId", "0" } };
        string testContent = await ClientHttpComponent.Instance.HttpPostAsync(ServerConfigComponent.Instance.GetWebUrl("test"), MiniJSON.Json.Serialize(dic));
        //Dictionary<string, string> dic = new Dictionary<string, string>() { { "action", "register" }, { "account", "gggg2" }, { "psw", "123" } };
        //HttpPostAsync(ServerConfigComponent.Instance.GetWebUrl("register"), dic, RegisterCallbackFunc);
        //Dictionary<string, string> dic = new Dictionary<string, string>() { { "action", "login" }, { "account", "gggg3" }, { "psw", "123" } };
        //HttpPostAsync(ServerConfigComponent.Instance.GetWebUrl("login"), dic, LoginCallbackFunc);

        //Dictionary<string, string> dic2 = new Dictionary<string, string>() { { "action", "activity" } };
        //string str = await HttpPostAsync("http://192.168.1.130:20002", dic2);

        //Dictionary<string, string> dic3 = new Dictionary<string, string>() { { "action", "activity" } };
        //HttpGetAsync("http://192.168.1.130:20002", dic3, LoginCallbackFunc);

        //Dictionary<string, string> dic2 = new Dictionary<string, string>() { { "type", "Activity" }, { "rrr2", "gggg2" } };
        //HttpPostAsync(ServerConfigManager.Instance.WebAddress, dic2, CallbackFunc);
        //Dictionary<string, string> dic3 = new Dictionary<string, string>() { { "type2", "Activity" }, { "rrr2", "gggg2" } };
        //HttpPostAsync(ServerConfigManager.Instance.WebAddress, dic3, CallbackFunc);
    }
    public void RegisterCallbackFunc(string content)
    {
        //LoginReply obj = (LoginReply)JsonConvert.DeserializeObject(content, typeof(LoginReply));
        //if(obj.ret == 0)
        //{
        //    Console.WriteLine(obj.id);
        //}
        //else
        //{
        //    Console.WriteLine("register fail.");
        //}
    }
    public void LoginCallbackFunc(string content)
    {
        //LoginReply obj = (LoginReply)JsonConvert.DeserializeObject(content, typeof(LoginReply));
        //if(obj.ret == 0)
        //{
        //    Console.WriteLine("login success.");
        //}
        //else
        //{
        //    Console.WriteLine("login fail.");
        //}
    }
    /// <summary>  
    /// GET方式的HTTP请求  
    /// </summary>  
    /// <param name="url">请求的URL</param>  
    /// <param name="timeout">请求的超时时间</param>  
    /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
    /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
    /// <returns></returns>  
    public string CreateGetHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent = null, CookieCollection cookies = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        string paramStr = GetParamStr(parameters);
        HttpWebRequest request = WebRequest.Create($"{url}?{paramStr}") as HttpWebRequest;
        request.Method = "GET";
        request.UserAgent = DefaultUserAgent;
        if (!string.IsNullOrEmpty(userAgent))
        {
            request.UserAgent = userAgent;
        }
        if (timeout.HasValue)
        {
            request.Timeout = timeout.Value;
        }
        if (cookies != null)
        {
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
        }

        string content = "";
        try
        {
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if(response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                content = responseReader.ReadToEnd();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return content;
    }
    /// <summary>  
    /// POST方式的HTTP请求  
    /// </summary>  
    /// <param name="url">请求的URL</param>  
    /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
    /// <param name="timeout">请求的超时时间</param>  
    /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
    /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
    /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
    /// <returns></returns>  
    public string CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, Encoding requestEncoding = null, string userAgent = null, CookieCollection cookies = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        if (requestEncoding == null)
        {
            throw new ArgumentNullException("requestEncoding");
        }
        HttpWebRequest request = null;
        //如果是发送HTTPS请求  
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
        }
        else
        {
            request = WebRequest.Create(url) as HttpWebRequest;
        }
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";

        if (!string.IsNullOrEmpty(userAgent))
        {
            request.UserAgent = userAgent;
        }
        else
        {
            request.UserAgent = DefaultUserAgent;
        }

        if (timeout.HasValue)
        {
            request.Timeout = timeout.Value;
        }
        if (cookies != null)
        {
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
        }
        //如果需要POST数据  
        if (parameters != null && parameters.Count > 0)
        {
            if (requestEncoding == null)
                requestEncoding = Encoding.UTF8;
            byte[] data = requestEncoding.GetBytes(GetParamStr(parameters));
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
        }
        string content = "";
        try
        {
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                content = responseReader.ReadToEnd();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        return content;
    }
    /// <summary>
    /// 异步Get
    /// </summary>
    /// <param name="address"></param>
    /// <param name="parameters"></param>
    /// <param name="handleFunc"></param>
    public async void HttpGetAsync(string address, IDictionary<string, string> parameters, Action<string> handleFunc)
    {
        WebClient wc = new WebClient();
        string paramStr = GetParamStr(parameters);
        Uri uri = new Uri($"{address}?{paramStr}", UriKind.RelativeOrAbsolute);
        try
        {
            Stream stream = await wc.OpenReadTaskAsync(uri);
            if (handleFunc != null)
            {
                StreamReader read = new StreamReader(stream);
                handleFunc(read.ReadToEnd());
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    /// <summary>
    /// 异步Post
    /// </summary>
    /// <param name="address"></param>
    /// <param name="parameters"></param>
    /// <param name="handleFunc"></param>
    public async Task<string> HttpPostAsync(string address, IDictionary<string, string> parameters)
    {
        Uri uri = new Uri(address);
        //ByteArrayContent content = null;
        HttpContent postContent = null;
        if (parameters != null && parameters.Count > 0)
        {
            //byte[] byteArray = Encoding.UTF8.GetBytes(GetParamStr(parameters));
            //content = new ByteArrayContent(byteArray);
            postContent = new FormUrlEncodedContent(parameters);
        }
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");   // HTTP KeepAlive设为false，防止HTTP连接保持
        try
        {
            HttpResponseMessage resp = await httpClient.PostAsync(uri, postContent);
            if(resp.StatusCode == HttpStatusCode.OK)
            {
                return await resp.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"HttpPostAsync fail({resp.StatusCode.ToString()}) to {address} .");
                return null;
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
    }

    /// <summary>
    /// 异步Post
    /// </summary>
    /// <param name="address"></param>
    /// <param name="parameters"></param>
    /// <param name="handleFunc"></param>
    public async Task<string> HttpPostAsync(string address, string content)
    {
        Uri uri = new Uri(address);
  
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");   // HTTP KeepAlive设为false，防止HTTP连接保持
        try
        {
            HttpContent httpContent = new StringContent(content);
            HttpResponseMessage resp = await httpClient.PostAsync(uri, httpContent);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                return await resp.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"HttpPostAsync fail({resp.StatusCode.ToString()}) to {address} .");
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
    }
    private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true; //总是接受  
    }
    private string GetParamStr(IDictionary<string, string> parameters)
    {
        if(parameters != null && parameters.Count > 0)
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                }
                i++;
            }
            return buffer.ToString();
        }
        else
        {
            return "";
        }
    }
}
