using System;
using System.Collections.Generic;
using Base;

public class ConstDefine
{
    public static int managerServerId = 100;
    public static int loginServerId = 1;
    public static Dictionary<AppType, int> initModuleDic = new Dictionary<AppType, int>()
    {
        { AppType.GameServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig },
        { AppType.LoginServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig },
        { AppType.GateServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig | (int)InitModule.DBConnect},
        { AppType.MapServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig },
        //{ AppType.TeamServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig },
        { AppType.BattleServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig | (int)InitModule.DBConnect},
        //{ AppType.ChatServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig },
        { AppType.ManagerServer, (int)InitModule.InnerConnect | (int)InitModule.LoadConfig | (int)InitModule.DBConnect},
        { AppType.SMITH, (int)InitModule.LoadConfig },
    };

    public static Dictionary<AppType, List<AppType>> innerConnectDic = new Dictionary<AppType, List<AppType>>()
    {
        { AppType.LoginServer, new List<AppType>() {AppType.ManagerServer}},
        { AppType.GateServer, new List<AppType>() {AppType.ManagerServer, AppType.BattleServer }},
        { AppType.BattleServer, new List<AppType>() { AppType.ManagerServer, AppType.GateServer}},
        { AppType.ManagerServer, new List<AppType>() {AppType.LoginServer, AppType.GateServer, AppType.BattleServer}}
    };
    public static string configPath = "../../Config/";
    public static string xmlPath = "../../Config/xml/";
    public static float fixedTime = 0.06f;
    public static long SynCharacterAttributeTime = 10;//毫秒
    public static long roomPingDec = 8000;
    public static float g = 10f;
    public static float shrink = 0.001f;
    public static int enlarge = 1000;
    public static float rebountTime = 0.2f;
    public static float reboundSpeed = 5f;
    public static float hitedTime = 2f;
    public static int max_lv = 5;
    public static int lowTimeTaskWaitTime = 10;
    public static float pickup_dis = 1f;
    public static float restMaxTime = 7;
    public static float findFoodDistance = 3f;
    public static float decFoodTime = 2f;
    public static float eatFoodDistance = 0.5f;
    public static float eatFoodTime = 2f;
    public static float viewTime = 3f;
    public static float synMoveTime = 1f;
}