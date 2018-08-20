using System;
using System.Collections.Generic;
using System.Xml;
using Base;

public class ServerConfig
{
    public int bigAreaId;
    public AppType appType;
    public int appid;
    public string innerip;
    public int innerport;
    public string listenOuterip;
    public int listenOuterport;
    public string outerip;
    public int outerport;
    public string innerAddress;
    public string outerAddress;
}
public class BigAreaConfig
{
    public Dictionary<int, ServerConfig> serverConfigDic = new Dictionary<int, ServerConfig>();
    public int bigAreaId;
    public string bigAreaName;
    public int worldAppId;
    public int chatAppId;
    public int dbAppId;
    public int managerAppId;
    public string gmHttpip;
    public int gmHttpport;
    public string playerDBUrl;
}

public class ServerConfigComponent : Component, IAwake
{
    public static ServerConfigComponent Instance;

    private Dictionary<string, string> webUrlDic = new Dictionary<string, string>();
    public Dictionary<int, BigAreaConfig> bigAreaCfDic = new Dictionary<int, BigAreaConfig>();
    private BigAreaConfig curBigAreaCf;
    private ServerConfig loginServerCf;
    #region
    private int loginAppId;
    public int LoginAppId { get { return loginAppId; } }
    private string loginHttpip;
    public string LoginHttpIp { get { return loginHttpip; } }
    private int loginHttpPort;
    public int LoginHttpPort { get { return loginHttpPort; } }

    public string BigAreaName { get { return curBigAreaCf.bigAreaName; } }
    public int WorldAppId { get { return curBigAreaCf.worldAppId; } }
    public int ChatAppId { get { return curBigAreaCf.chatAppId; } }
    public int ManagerAppId { get { return curBigAreaCf.managerAppId; } }
    public string GmHttpIp { get { return curBigAreaCf.gmHttpip; } }
    public int GmHttpPort { get { return curBigAreaCf.gmHttpport; } }
    public string PlayerDBUrl { get { return curBigAreaCf.playerDBUrl; } }
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
    public bool LoadConfig(int bigAreaId)
    {
        bigAreaCfDic.Clear();

        XmlDocument xmlDoc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        XmlReader reader = XmlReader.Create("../../serverconfig.xml", settings);
        xmlDoc.Load(reader);

        XmlNode rootNode = xmlDoc.SelectSingleNode("Root");

        XmlNode loginNode = rootNode.SelectSingleNode("LoginServer");
        if (loginNode == null)
        {
            Console.WriteLine("server config LoginServer is null.");
            return false;
        }
        XmlElement loginElem = (XmlElement)loginNode;
        loginServerCf = Analysis(loginElem, AppType.LoginServer);
        loginAppId = loginServerCf.appid;

        XmlNode projectNode = rootNode.SelectSingleNode("project");
        if (projectNode == null)
        {
            Console.WriteLine("server config project is null.");
            return false;
        }
        XmlElement projectElem = (XmlElement)projectNode;
        projectid = int.Parse(projectElem.GetAttribute("id"));

        XmlNode loginHttpNode = rootNode.SelectSingleNode("LoginHttp");
        if(loginHttpNode == null)
        {
            Console.WriteLine("server config LoginHttp is null.");
            return false;
        }
        XmlElement loginHttpElem = (XmlElement)loginHttpNode;
        loginHttpip = loginHttpElem.GetAttribute("ip");
        loginHttpPort = int.Parse(loginHttpElem.GetAttribute("port"));

        XmlNode webNode = rootNode.SelectSingleNode("WebUrl");
        if (webNode == null)
        {
            Console.WriteLine("server config WebUrl is null.");
            return false;
        }
        XmlNodeList urlNodes = webNode.SelectNodes("url");
        if(urlNodes == null)
        {
            Console.WriteLine("server config url is null.");
            return false;
        }
        foreach(XmlNode urlNode in urlNodes)
        {
            XmlElement rulElem = (XmlElement)urlNode;
            string actionName = rulElem.GetAttribute("action");
            string actionAddress = rulElem.GetAttribute("address");
            if(webUrlDic.ContainsKey(actionName))
            {
                Console.WriteLine("server config weburl exist same action.");
                return false;
            }
            else
            {
                webUrlDic.Add(actionName, actionAddress);
            }
        }

        XmlNodeList bigAreaNodes = rootNode.SelectNodes("BigArea");
        if(bigAreaNodes == null)
        {
            Console.WriteLine("server config bigArea is null.");
            return false;
        }
        
        if (bigAreaId > 0)
        {
            XmlNode bigAreaNode = null;
            foreach (XmlNode node in bigAreaNodes)
            {
                int areaId = int.Parse(((XmlElement)node).GetAttribute("id"));
                if (areaId == bigAreaId)
                {
                    bigAreaNode = node;
                    break;
                }
            }
            if (bigAreaNode == null)
            {
                Console.WriteLine($"Not exist bigAreaId({bigAreaId}) in server config.");
                return false;
            }
            bool isSuccess = LoadBigArea(bigAreaNode);
            if (!isSuccess)
                return false;
            curBigAreaCf = bigAreaCfDic[bigAreaId];
        }
        else if(loginAppId == Game.Instance.Appid)
        {
            foreach (XmlNode node in bigAreaNodes)
            {
                bool isSuccess = LoadBigArea(node);
                if (!isSuccess)
                    return false;
            }
        }
        else
        {
            Console.WriteLine($"input bigAreaId({bigAreaId}), and input Appid({Game.Instance.Appid}) not same with LoginServer({loginAppId}) in server config.");
            return false;
        }
        return true;
    }
    private bool LoadBigArea(XmlNode bigAreaNode)
    {
        BigAreaConfig areaCf = new BigAreaConfig();
        XmlElement elem = (XmlElement)bigAreaNode;
        areaCf.bigAreaName = elem.GetAttribute("name");
        areaCf.bigAreaId = int.Parse(elem.GetAttribute("id"));

        ServerConfig serverConfig = null;
        //XmlNode dbNode = bigAreaNode.SelectSingleNode("DBServer");
        //if (dbNode != null)
        //{
        //    elem = (XmlElement)dbNode;
        //    serverConfig = Analysis(elem, AppType.DBServer);
        //    areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
        //    areaCf.dbAppId = serverConfig.appid;
        //    //Console.WriteLine($"Not exist DBServer in bigArea({areaCf.bigAreaId}) server config.");
        //    //return false;
        //}

        XmlNode SmithNode = bigAreaNode.SelectSingleNode("SMITH");
        if (SmithNode != null)
        {
            elem = (XmlElement)SmithNode;
            serverConfig = Analysis(elem, AppType.SMITH);
            areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
            //Console.WriteLine($"Not exist DBServer in bigArea({areaCf.bigAreaId}) server config.");
            //return false;
        }


        XmlNode gatesNode = bigAreaNode.SelectSingleNode("Gates");
        if (gatesNode != null)
        {
            XmlNodeList nodes = gatesNode.SelectNodes("GateServer");
            if(nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    elem = (XmlElement)node;
                    serverConfig = Analysis(elem, AppType.GateServer);
                    areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
                }
            }
            else
            {
                Console.WriteLine($"Not exist GateServer in bigArea({areaCf.bigAreaId}) server config.");
                return false;
            }
        }
        else
        {
            Console.WriteLine($"Not exist Gates in bigArea({areaCf.bigAreaId}) server config.");
            return false;
        }

        XmlNode gamesNode = bigAreaNode.SelectSingleNode("Games");
        if (gamesNode != null)
        {
            XmlNodeList nodes = gamesNode.SelectNodes("GameServer");
            if(nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    elem = (XmlElement)node;
                    serverConfig = Analysis(elem, AppType.GameServer);
                    areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
                }
            }
            else
            {
                Console.WriteLine($"Not exist GameServer in bigArea({areaCf.bigAreaId}) server config.");
                return false;
            }
        }
        //else
        //{
        //    Console.WriteLine($"Not exist Games in bigArea({areaCf.bigAreaId}) server config.");
        //    return false;
        //}


        //XmlNode chatNode = bigAreaNode.SelectSingleNode("ChatServer");
        //if (chatNode != null)
        //{
        //    elem = (XmlElement)chatNode;
        //    serverConfig = Analysis(elem, AppType.ChatServer);
        //    areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
        //    areaCf.chatAppId = serverConfig.appid;
        //    //Console.WriteLine($"Not exist ChatServer in bigArea({areaCf.bigAreaId}) server config.");
        //    //return false;
        //}


        XmlNode worldsNode = bigAreaNode.SelectSingleNode("WorldServer");
        if (worldsNode != null)
        {
            elem = (XmlElement)worldsNode;
            serverConfig = Analysis(elem, AppType.MapServer);
            areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
            areaCf.worldAppId = serverConfig.appid;
            //Console.WriteLine($"Not exist WorldServer in bigArea({areaCf.bigAreaId}) server config.");
            //return false;
        }


        XmlNode battleNode = bigAreaNode.SelectSingleNode("Battles");
        if (battleNode != null)
        {
            string connectApp = ((XmlElement)battleNode).GetAttribute("connect");
            XmlNodeList nodes = battleNode.SelectNodes("BattleServer");
            if(nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    elem = (XmlElement)node;
                    serverConfig = Analysis(elem, AppType.BattleServer);
                    areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
                }
            }
            else
            {
                Console.WriteLine($"Not exist BattleServer in bigArea({areaCf.bigAreaId}) server config.");
                return false;
            }
        }
        //else
        //{
        //    Console.WriteLine($"Not exist Battles in bigArea({areaCf.bigAreaId}) server config.");
        //    return false;
        //}

        //XmlNode scenesNode = bigAreaNode.SelectSingleNode("Scenes");
        //if (scenesNode != null)
        //{
        //    string connectApp = ((XmlElement)scenesNode).GetAttribute("connect");
        //    XmlNodeList nodes = scenesNode.SelectNodes("SceneServer");
        //    if(nodes != null && nodes.Count > 0)
        //    {
        //        foreach (XmlNode node in nodes)
        //        {
        //            elem = (XmlElement)node;
        //            serverConfig = Analysis(elem, AppType.TeamServer);
        //            areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Not exist SceneServer in bigArea({areaCf.bigAreaId}) server config.");
        //        return false;
        //    }
        //}
        //else
        //{
        //    Console.WriteLine($"Not exist Scenes in bigArea({areaCf.bigAreaId}) server config.");
        //    return false;
        //}

        XmlNode managerNode = bigAreaNode.SelectSingleNode("ManagerServer");
        if (managerNode != null)
        {
            elem = (XmlElement)managerNode;
            serverConfig = Analysis(elem, AppType.ManagerServer);
            areaCf.serverConfigDic.Add(serverConfig.appid, serverConfig);
            areaCf.managerAppId = serverConfig.appid;
            //Console.WriteLine($"Not exist ManagerServer in bigArea({areaCf.bigAreaId}) server config.");
            //return false;
        }


        XmlNode httpNode = bigAreaNode.SelectSingleNode("GmHttp");
        if (httpNode == null)
        {
            Console.WriteLine($"Not exist GmHttp in bigArea({areaCf.bigAreaId}) server config.");
            return false;
        }
        elem = (XmlElement)httpNode;
        areaCf.gmHttpip = elem.GetAttribute("ip");
        areaCf.gmHttpport = int.Parse(elem.GetAttribute("port"));

        XmlNode playerDBUrlNode = bigAreaNode.SelectSingleNode("PlayerDB");
        if (playerDBUrlNode == null)
        {
            Console.WriteLine($"Not exist PlayerDB in bigArea({areaCf.bigAreaId}) server config.");
            return false;
        }
        elem = (XmlElement)playerDBUrlNode;
        areaCf.playerDBUrl = elem.GetAttribute("address");

        bigAreaCfDic.Add(areaCf.bigAreaId, areaCf);

        return true;
    }
    private ServerConfig Analysis(XmlElement elem, AppType appType)
    {
        ServerConfig serverConfig = new ServerConfig();
        serverConfig.appid = int.Parse(elem.GetAttribute("appid"));
        serverConfig.appType = appType;
        serverConfig.innerip = elem.GetAttribute("innerip");
        string str;
        str = elem.GetAttribute("innerport");
        if (!string.IsNullOrEmpty(str))
            serverConfig.innerport = int.Parse(str);
        serverConfig.innerAddress = serverConfig.innerip + ":" + serverConfig.innerport;
        serverConfig.outerip = elem.GetAttribute("outerip");
        str = elem.GetAttribute("outerport");
        if (!string.IsNullOrEmpty(str))
            serverConfig.outerport = int.Parse(str);
        serverConfig.outerAddress = serverConfig.outerip + ":" + serverConfig.outerport;

        serverConfig.listenOuterip = elem.GetAttribute("listenOuterip");
        str = elem.GetAttribute("listenOuterport");
        if (!string.IsNullOrEmpty(str))
            serverConfig.listenOuterport = int.Parse(str);

        return serverConfig;
    }
    public ServerConfig GetServerConfigByAppid(int appid)
    {
        if (loginAppId == appid)
            return loginServerCf;
        ServerConfig serverConfig = null;
        if(curBigAreaCf == null)
        {
            foreach (BigAreaConfig bigArea in bigAreaCfDic.Values)
            {
                foreach (ServerConfig sc in bigArea.serverConfigDic.Values)
                {
                    if (sc.appid == appid)
                    {
                        serverConfig = sc;
                        break;
                    }
                }
            }
        }
        else
        {
            curBigAreaCf.serverConfigDic.TryGetValue(appid, out serverConfig);
        }
        return serverConfig;
    }
    public ServerConfig GetServerConfigByAppid(int bigAreaid, int appid)
    {
        BigAreaConfig bigAreaCf = null;
        ServerConfig serverConfig = null;
        if (bigAreaCfDic.TryGetValue(bigAreaid, out bigAreaCf))
        {
            bigAreaCf.serverConfigDic.TryGetValue(appid, out serverConfig);
        }
        return serverConfig;
    }
    public List<ServerConfig> GetServerConfigByAppType(AppType appType)
    {
        List<ServerConfig> serverConfigs = new List<ServerConfig>();
        if (appType == AppType.LoginServer)
            serverConfigs.Add(loginServerCf);
        else
        {
            foreach(BigAreaConfig bigArea in bigAreaCfDic.Values)
            {
                foreach (ServerConfig serverConfig in bigArea.serverConfigDic.Values)
                {
                    if (serverConfig.appType == appType)
                        serverConfigs.Add(serverConfig);
                }
            }
        }
        return serverConfigs;
    }
    public string GetWebUrl(string actionName)
    {
        string url = null;
        webUrlDic.TryGetValue(actionName, out url);
        return url;
    }
    public BigAreaConfig GetBigAreaCfById(int bigAreaid)
    {
        BigAreaConfig bigAreaCf = null;
        bigAreaCfDic.TryGetValue(bigAreaid, out bigAreaCf);
        return bigAreaCf;
    }
}
