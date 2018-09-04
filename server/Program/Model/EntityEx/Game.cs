using System;
using System.IO;
using System.Net;
using Base;
using Data;

public enum GameState
{
    Starting,
    Runing,
    WaitEnd,
    End
}
public class Game : Entity
{
    static Game instance;
    public static Game Instance
    {
        get
        {
            if (instance == null)
                instance = new Game();
            return instance;
        }
    }
    private int appid;
    public int Appid { get { return appid; } }

    private int bigAreaId;
    public int BigAreaId { get { return bigAreaId; } }

    private AppType appType;
    public AppType AppType { get { return appType; } }

    private GameState state;
    public GameState State { get { return state; } }
    
    private long msec;
    public long Msec { get { return msec; } }
    private long sec;
    public long Sec { get { return sec; } }
    private int initModule;

    Game() : base(EntityType.Game)
    {

    }
    public void Init(int appid, int bigAreaId)
    {
        this.appid = appid;
        this.bigAreaId = bigAreaId;
        this.state = GameState.Starting;
        IdGenerater.AppId = appid;
        msec = TimeHelper.ClientNow();
        sec = TimeHelper.ClientNowSeconds();
        initModule = 0;
        //服务器配置
        AddComponent<ServerConfigComponent>();
        bool isLoadSuccess = ServerConfigComponent.Instance.LoadConfig(appid, bigAreaId);
        if(!isLoadSuccess)
        {
            this.state = GameState.End;
            Console.WriteLine($"error : serverconfig load fail.");
            return;
        }
        ServerConfig serverConfig = ServerConfigComponent.Instance.curServerConfig;
        if (serverConfig == null)
        {
            this.state = GameState.End;
            Console.WriteLine($"error : appid {appid} not find in serverconfig.");
            return;
        }
        appType = serverConfig.appType;
        Console.WriteLine($"***** {Appid} {appType} Starting. ******");
        Log.Init(appType, appid);
        LoadComponent();
    }

    public void Update()
    {
        msec = TimeHelper.ClientNow();
        sec = TimeHelper.ClientNowSeconds();
        ComponentManager.Instance.Update();
    }
    public void LoadComponent()
    {
        ServerConfig serverConfig = ServerConfigComponent.Instance.curServerConfig;

        AddComponent<ConstConfigComponent>();
        AddComponent<ConfigManagerComponent>();
        AddComponent<TimeManagerComponent>();
        AddComponent<HitfixComponent>();
        AddComponent<XmlDataComponent>();
        switch (appType)
        {
            case AppType.SMITH:
                //AddComponent<SmithComponent, string, int, AppType>( serverConfig.outerip, serverConfig.outerport, appType);
                break;
            case AppType.LoginServer:
                AddComponent<NetInnerComponent, IPEndPoint, AppType>(serverConfig.inner, appType);
                AddComponent<NetOuterComponent, IPEndPoint, AppType>(serverConfig.listenOuter, appType);
                //AddComponent<HttpComponent, string, int>(ServerConfigComponent.Instance.LoginHttpIp, ServerConfigComponent.Instance.LoginHttpPort);
                AddComponent<ClientHttpComponent>();
                AddComponent<LoginManagerComponent>();
                break;
            case AppType.GameServer:
                AddComponent<DBOperateComponent>();
                AddComponent<NetInnerComponent, IPEndPoint, AppType>(serverConfig.inner, appType);
                break;
            case AppType.GateServer:
                AddComponent<NetInnerComponent, IPEndPoint, AppType>(serverConfig.inner, appType);
                AddComponent<NetOuterComponent, IPEndPoint, AppType>(serverConfig.listenOuter, appType);
                AddComponent<VerifyComponent>();
                AddComponent<PlayerManagerComponent>();
                AddComponent<ClientHttpComponent>();
                break;
            case AppType.MapServer:
                AddComponent<NetInnerComponent, IPEndPoint, AppType>(serverConfig.inner, appType);
                break;
            case AppType.SystemServer:
                {
                    AddComponent<DBOperateComponent>();
                    AddComponent<NetInnerComponent, IPEndPoint, AppType>(serverConfig.inner, appType);
                    if ((serverConfig.system & (int)SystemType.Chat) > 0)
                    {

                    }
                    if((serverConfig.system & (int)SystemType.Guild) > 0)
                    {

                    }
                    if ((serverConfig.system & (int)SystemType.Mail) > 0)
                    {

                    }
                    if ((serverConfig.system & (int)SystemType.Team) > 0)
                    {

                    }
                    if ((serverConfig.system & (int)SystemType.Friend) > 0)
                    {

                    }
                }
                break;
            case AppType.BattleServer:
                AddComponent<NetInnerComponent, IPEndPoint, AppType>(serverConfig.inner, appType);
                //AddComponent<NetOuterComponent, string, int, AppType>(serverConfig.listenOuterip, serverConfig.listenOuterport, appType);
                AddComponent<RoomManagerComponent>();
                break;
            case AppType.ManagerServer:
                AddComponent<NetInnerComponent, IPEndPoint, AppType>(serverConfig.inner, appType);
                //AddComponent<HttpComponent, string, int>(ServerConfigComponent.Instance.GmHttpIp, ServerConfigComponent.Instance.GmHttpPort);
                AddComponent<GmManagerComponent>();
                AddComponent<ClientHttpComponent>();
                AddComponent<ServerAllotComponent>();
                AddComponent<RoomAllotComponent>();
                AddComponent<DBOperateComponent>();
                break;
        }
    }

    public void SetInitFinishModule(InitModule moduleType)
    {
        if(state != GameState.Starting)
        {
            return;
        }
        initModule |= (int)moduleType;
        Console.WriteLine("Finish module : " + moduleType.ToString());
        if(ConstDefine.initModuleDic[appType] == initModule)
        {
            state = GameState.Runing;
            Console.WriteLine("启动成功");
        }
    }
    public bool IsFinishModule(InitModule moduleType)
    {
        return (initModule & (int)moduleType) > 0;
    }
}
