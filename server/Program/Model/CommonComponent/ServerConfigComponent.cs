using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Base;

public class ServerConfig
{
    //public int bigAreaId;
    public AppType appType;
    public int appid;
    //public string innerip;
    //public int innerport;
    //public string listenOuterip;
    //public int listenOuterport;
    public string outerip;
    public int outerport;
    //public string innerAddress;
    //public string outerAddress;

    public IPEndPoint inner;
    public IPEndPoint listenOuter;

    public int system;
}

public class ServerConfigComponent : Component, IAwake
{
    public static ServerConfigComponent Instance;

    private Dictionary<string, string> webUrlDic = new Dictionary<string, string>();

    public ServerConfig managerServerConfig;
    public ServerConfig curServerConfig;
    public ServerConfig loginServerConfig;

    private int areaId;
    private string areaName;
    private string gmHttpip;
    private int gmHttpport;
    private string dbUrl;
 
    #region
    public int LoginAppId { get { return loginServerConfig.appid; } }
    private string loginHttpip;
    public string LoginHttpIp { get { return loginHttpip; } }
    private int loginHttpPort;
    public int LoginHttpPort { get { return loginHttpPort; } }
    public int ManagerAppId { get { return managerServerConfig.appid; } }
    public string GmHttpIp { get { return gmHttpip; } }
    public int GmHttpPort { get { return gmHttpport; } }
    public string DBUrl { get { return dbUrl; } }
    private int projectid;
    public int Projectid { get { return projectid; } }
    #endregion

    public void Awake()
    {
        Instance = this;
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
    public bool LoadConfig(int curAppId, int curBigAreaId)
    {
        //bigAreaCfDic.Clear();

        XmlDocument xmlDoc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        XmlReader reader = XmlReader.Create("../../serverconfig.xml", settings);
        xmlDoc.Load(reader);

        XmlNode rootNode = xmlDoc.SelectSingleNode("Root");

        XmlNode loginNode = rootNode.SelectSingleNode("App");
        if (loginNode != null)
        {
            XmlElement loginElem = (XmlElement)loginNode;
            loginServerConfig = Analysis(loginElem);

            XmlNode loginHttpNode = rootNode.SelectSingleNode("LoginHttp");
            if (loginHttpNode != null)
            {
                XmlElement loginHttpElem = (XmlElement)loginHttpNode;
                loginHttpip = loginHttpElem.GetAttribute("ip");
                loginHttpPort = int.Parse(loginHttpElem.GetAttribute("port"));
            }
        }

        XmlNode projectNode = rootNode.SelectSingleNode("project");
        if (projectNode == null)
        {
            Console.WriteLine("server config project is null.");
            return false;
        }
        XmlElement projectElem = (XmlElement)projectNode;
        projectid = int.Parse(projectElem.GetAttribute("id"));

        XmlNode webNode = rootNode.SelectSingleNode("WebUrl");
        if (webNode != null)
        {
            XmlNodeList urlNodes = webNode.SelectNodes("url");
            if (urlNodes != null)
            {
                foreach (XmlNode urlNode in urlNodes)
                {
                    XmlElement rulElem = (XmlElement)urlNode;
                    string actionName = rulElem.GetAttribute("action");
                    string actionAddress = rulElem.GetAttribute("address");
                    if (webUrlDic.ContainsKey(actionName))
                    {
                        Console.WriteLine("server config weburl exist same action.");
                        return false;
                    }
                    else
                    {
                        webUrlDic.Add(actionName, actionAddress);
                    }
                }

            }
        }
        
        if (curBigAreaId > 0)
        {
            XmlNodeList bigAreaNodes = rootNode.SelectNodes("BigArea");
            if (bigAreaNodes == null)
            {
                Console.WriteLine($"server config bigArea({curBigAreaId}) is null.");
                return false;
            }

            XmlNode bigAreaNode = null;
            foreach (XmlNode node in bigAreaNodes)
            {
                int areaId = int.Parse(((XmlElement)node).GetAttribute("id"));
                if (areaId == curBigAreaId)
                {
                    bigAreaNode = node;
                    break;
                }
            }
            if (bigAreaNode == null)
            {
                Console.WriteLine($"Not exist bigAreaId({curBigAreaId}) in server config.");
                return false;
            }

            XmlElement areaNode = bigAreaNode as XmlElement;
            areaName = areaNode.GetAttribute("name");
            areaId = curBigAreaId;

            XmlNodeList appNodes = bigAreaNode.SelectNodes("App");

            XmlNode managerNode = null;
            XmlNode curServerNode = null;
            foreach (XmlNode node in appNodes)
            {
                int appId = int.Parse(((XmlElement)node).GetAttribute("appid"));
                if (appId == curAppId)
                {
                    curServerNode = node;
                }
                if(appId == ConstDefine.managerServerId)
                {
                    managerNode = node;
                }
            }
            if(curServerNode == null)
            {
                Console.WriteLine($"server config not exist app({curAppId}) in bigArea({curBigAreaId}).");
                return false;
            }
            if (managerNode == null)
            {
                Console.WriteLine($"server config not exist managerServer in bigArea({curBigAreaId}).");
                return false;
            }
            if(curServerNode == managerNode)
            {
                curServerConfig = Analysis((XmlElement)curServerNode);
                managerServerConfig = curServerConfig;
            }
            else
            {
                curServerConfig = Analysis((XmlElement)curServerNode);
                managerServerConfig = Analysis((XmlElement)managerNode);
            }

            XmlElement httpNode = bigAreaNode.SelectSingleNode("GmHttp") as XmlElement;
            if(httpNode != null)
            {
                string ip = httpNode.GetAttribute("innerip");
                string str = httpNode.GetAttribute("innerport");
                int port = 0;
                if (int.TryParse(str, out port))
                {
                    gmHttpip = ip;
                    gmHttpport = port;
                }
            }
            XmlElement dbNode = bigAreaNode.SelectSingleNode("DB") as XmlElement;
            if(dbNode != null)
            {
                dbUrl = httpNode.GetAttribute("address");
            }
        }

        return true;
    }
    private ServerConfig Analysis(XmlElement elem)
    {
        ServerConfig serverConfig = new ServerConfig();

        serverConfig.appid = int.Parse(elem.GetAttribute("appid"));

        string str = elem.GetAttribute("apptype");
        serverConfig.appType = (AppType)Enum.Parse(typeof(AppType), str);

        string ip = elem.GetAttribute("innerip");
        str = elem.GetAttribute("innerport");
        int port = 0;
        if(int.TryParse(str, out port))
        {
            serverConfig.inner = NetworkHelper.ToIPEndPoint(ip, port);
        }
        
        ip = elem.GetAttribute("outerip");
        str = elem.GetAttribute("outerport");
        if (String.IsNullOrEmpty(str) == false && int.TryParse(str, out port))
        {
            serverConfig.outerip = ip;
            serverConfig.outerport = port;
        }

        ip = elem.GetAttribute("listenOuterip");
        str = elem.GetAttribute("listenOuterport");
        if (String.IsNullOrEmpty(str) == false && int.TryParse(str, out port))
        {
            serverConfig.listenOuter = NetworkHelper.ToIPEndPoint(ip, port);
        }

        if(serverConfig.appType == AppType.SystemServer)
        {
            str = elem.GetAttribute("system");
            if (String.IsNullOrEmpty(str) == false)
            {
                string[] strs = str.Split('|');
                for(int i = 0; i < strs.Length; i++)
                {
                   int system = (int)Enum.Parse(typeof(SystemType), strs[i]);
                    if((system & serverConfig.system) == 0)
                    {
                        serverConfig.system |= system;
                    }
                }
            }
            else
            {
                Console.WriteLine($"Not exist system in system server({serverConfig.appid}).");
            }
        }


        return serverConfig;
    }
    public string GetWebUrl(string actionName)
    {
        string url = null;
        webUrlDic.TryGetValue(actionName, out url);
        return url;
    }
}
