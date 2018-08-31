namespace Base
{
    public enum AppType
    {
        Client,
        //单开s
        ManagerServer,
        LoginServer,
        //多开
        GateServer = 10,
        MapServer,
        GameServer,
        BattleServer,
        //可拆分系统,工会、好友、邮件、聊天、组队...
        SystemServer = 100,
        All,
        SMITH,
    }
    //系统模块
    public enum SystemType
    {
        None = 0,
        Guild = 1 << 1,
        Friend = 1 << 2,
        Mail = 1 << 3,
        Chat = 1 << 4,
        Team = 1 << 5
    }
}
