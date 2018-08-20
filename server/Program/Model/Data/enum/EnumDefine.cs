public enum InitModule
{
    None = 0,
    LoadConfig = 1,
    InnerConnect = 2,
    DBConnect = 4,
    DB_DataRefresh = 8,
    GetActivity = 16
}
public enum HotfixModule
{
    Protocol = 1,
    Event = 2,
    Http = 4,
    Func = 8
}
public enum LogType
{
    roomcard
}
public enum LogAction
{
    None,
    gm_fillcard,//充房卡
    AA_costcard,//AA支付
    fangzhu_costcard,//房主支付
    loginactivity_getcard,//登录活动获得房卡
    day7activity_getcard,//七日登录活动获得房卡
    shareactivity_getcard,//分享活动获得房卡
    bing_getcard,//绑定代理获得房卡
}