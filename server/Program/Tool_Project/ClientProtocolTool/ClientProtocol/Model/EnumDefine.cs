namespace Model
{
    public enum AppType
    {
        Client,
        GameServer,
        LoginServer,
        GateServer,
        DBServer,
        WorldServer,
        SceneServer,
        BattleServer,
        ChatServer,
        ManagerServer,
        All
    }
    //玩家详细属性
    public enum D_AttributeType
    {
        Max
    }
    //玩家额外属性
    public enum E_AttributeType
    {
        Max
    }
    public enum AttributeMaxValue
    {
        D_Attribute = D_AttributeType.Max,
        E_Attribute = E_AttributeType.Max,
        Max
    }
    public enum ErrorCode
    {
        Success = 0,
        CodeError,      //代码错误
        Fail,           //失败
        Processing,     //正在处理
        Online,         //在线
        NotExistPlayer,//玩家不存在
        NotExistScene,  //场景不存在
        SceneOutside,   //超出场景范围
        Exist,
    }
    public enum MemberState
    {
        Idle,
        Ready,
        Deal,
        Finsh
    }
}