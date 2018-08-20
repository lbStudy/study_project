using System;
using System.Collections.Generic;
using Base;
using Data;

public class PlayerAllotInfo
{
    public long playerid;
    public int gateid;
}

public class ServerAllotComponent : Component, IAwake
{
    public static ServerAllotComponent Instance;
    private Dictionary<int, int> gateNumDic = new Dictionary<int, int>();

    private Dictionary<long, PlayerAllotInfo> playerAllotInfoDic = new Dictionary<long, PlayerAllotInfo>();
    public void Awake()
    {
        Instance = this;
        List<ServerConfig> gateCfs = ServerConfigComponent.Instance.GetServerConfigByAppType(AppType.GateServer);
        for(int i = 0; i < gateCfs.Count; i++)
        {
            gateNumDic[gateCfs[i].appid] = 0;
        }
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
    public int TotalPlayerNumber { get { return playerAllotInfoDic.Count; } }
    public int GetGateApp()
    {
        int gateId = 0;
        int minNum = int.MaxValue;
        foreach(KeyValuePair<int, int> pair in gateNumDic)
        {
            if(pair.Value < minNum)
            {
                gateId = pair.Key;
                minNum = pair.Value;
            }
        }
        return gateId;
    }
    public void PlayerEnter(long playerid, int gateid)
    {
        PlayerAllotInfo info = null;
        if (playerAllotInfoDic.TryGetValue(playerid, out info))
        {
            info.gateid = gateid;
        }
        else
        {
            gateNumDic[gateid]++;
            info = new PlayerAllotInfo();
            info.playerid = playerid;
            info.gateid = gateid;
            playerAllotInfoDic[info.playerid] = info;
        }
    }
    public PlayerAllotInfo Find(long playerid)
    {
        PlayerAllotInfo info = null;
        playerAllotInfoDic.TryGetValue(playerid, out info);
        return info;
    }
    public void PlayerRemove(long playerid)
    {
        PlayerAllotInfo allotInfo = null;
        if (playerAllotInfoDic.TryGetValue(playerid, out allotInfo))
        {
            gateNumDic[allotInfo.gateid]--;
            playerAllotInfoDic.Remove(playerid);
        }
    }
}
