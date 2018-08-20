namespace Data
{
    public enum ErrorCode
    {
        Success = 0,
        CodeError,      //代码错误
        Fail,           //失败
        Processing,     //正在处理
        Online,         //在线
        NotExistPlayer, //玩家不存在
        NotExistScene,  //场景不存在
        Outside,        //范围外
        Exist,
        NotExist,
        Fill,
        DB_Disconnect,
        MatchFail,
        NotInBattle,
        NotEnough,
        InBattle,
        End
    }
}
