namespace Base
{
    public enum AppType
    {
        Client,
        //单开s
        ManagerServer,
        LoginServer,
        //多开
        GateServer,
        MapServer,
        GameServer,
        BattleServer,
        //可拆分系统,工会、好友、邮件、聊天、组队...
        SystemServer,
        All,
        SMITH,
    }
    //系统模块
    public enum SystemType
    {
        None,
        Guild,
        Friend,
        Mail,
        Chat,
        Team
    }
}
