using System;
using System.Collections.Generic;
using Base;
using Data;

public class RoomInfo
{
    public long roomid;
    public int battleAppid;
    public int curCount;
    public int totalCount;
    public int sceneid;
    public long canJoinTime;
    public List<MatchInfo> members = new List<MatchInfo>();
}
public class MatchInfo
{
    public long playerid;
    public int fromAppId;
    public long roomid;
    public MatchSynInfo sysInfo;
}
public class RoomAllotComponent : Component, IAwake
{
    public static RoomAllotComponent Instance;
    private Dictionary<int, int> numDic = new Dictionary<int, int>();
    private Dictionary<long, RoomInfo> roomDic = new Dictionary<long, RoomInfo>();

    private List<MatchInfo> matchingPlayers = new List<MatchInfo>();

    public Random random = new Random();
    public void Awake()
    {
        Instance = this;
        //List<ServerConfig> battleCfs = ServerConfigComponent.Instance.GetServerConfigByAppType(AppType.BattleServer);
        //for (int i = 0; i < battleCfs.Count; i++)
        //{
        //    numDic[battleCfs[i].appid] = 0;
        //}

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
    
    public long CreateRoomId()
    {
        long roomid = random.Next(100000, 999999);
        while (roomDic.ContainsKey(roomid))
        {
            int v = random.Next(1, 1000);
            if (roomid + v > 999999)
            {
                roomid -= v;
            }
            else
            {
                roomid += v;
            }
        }
        
        return roomid;
    }
    public int GetBattleServer()
    {
        int appId = 0;
        int minCount = int.MaxValue;
        foreach(KeyValuePair<int, int> pair in numDic)
        {
            if(minCount > pair.Value)
            {
                minCount = pair.Value;
                appId = pair.Key;
            }
        }
        return appId;
    }
    public void JoinBattleServer(RoomInfo info, long roomid)
    {
        numDic[info.battleAppid]++;
        roomDic.Add(roomid, info);
    }
    public void RemoveRoom(int appid, long roomid)
    {
        numDic[appid]--;
        roomDic.Remove(roomid);
    }
    public RoomInfo FindRoom(long roomid)
    {
        RoomInfo info = null;
        roomDic.TryGetValue(roomid, out info);
        return info;
    }
    public RoomInfo GetRandomRoom()
    {
        foreach(RoomInfo v in roomDic.Values)
        {
            if(v.curCount < v.totalCount)
            {
                return v;
            }
        }
        return null;
    }
    public void AddMatchPlayer(MatchInfo info)
    {
        matchingPlayers.Add(info);
    }
    public bool IsExistMatchPlayer(long playerid)
    {
        return matchingPlayers.Find(x => x.playerid == playerid) != null;
    }
    public int RemvoeMatchPlayer(long playerid)
    {
        return matchingPlayers.RemoveAll(x => x.playerid == playerid);
    }
}

