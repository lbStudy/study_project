using System;
using System.Collections.Generic;
using Data;
using Base;

[Pool]
public class VerifyInfo : Disposer
{
    public long playerid;
    public string checkCode;
    public long loseTime;
}
public class VerifyComponent : Component, IAwake
{
    public static VerifyComponent Instance;

    Dictionary<long, VerifyInfo> verifyPlayerDic = new Dictionary<long, VerifyInfo>();
    List<VerifyInfo> verifyPlayers = new List<VerifyInfo>();

    List<long> removeIds = new List<long>();
    public void Awake()
    {
        Instance = this;
        TimeTask timeTask = new IntervalTask(3000, TimeDetection);
        TimeManagerComponent.Instance.Add(timeTask);
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
    public void TimeDetection()
    {
        removeIds.Clear();
        for (int i = verifyPlayers.Count - 1; i >= 0; i--)
        {
            if(Game.Instance.Msec > verifyPlayers[i].loseTime)
            {
                removeIds.Add(verifyPlayers[i].playerid);
                Remove(verifyPlayers[i].playerid);
               
            }
        }
        if(removeIds.Count > 0)
        {
            Gate2M_VerifyTimeOutMessage msg = new Gate2M_VerifyTimeOutMessage();
            msg.playerids = removeIds;
            NetHelper.SendMessage(msg, null);
        }
    }
    public VerifyInfo Add(long playerid)
    {
        VerifyInfo info = ObjectPoolManager.Instance.Take<VerifyInfo>();
        info.playerid = playerid;
        info.checkCode = Guid.NewGuid().ToString();
        info.loseTime = Game.Instance.Msec + ConstConfigComponent.ConstConfig.GateVerifyWaitTime;
        verifyPlayerDic[playerid] = info;
        verifyPlayers.Add(info);
        return info;
    }
    public void Remove(long playerid)
    {
        VerifyInfo info = null;
        if(verifyPlayerDic.TryGetValue(playerid, out info))
        {
            verifyPlayerDic.Remove(playerid);
            verifyPlayers.Remove(info);
            info.Dispose();
        }
    }
    public bool IsExist(long playerid)
    {
        return verifyPlayerDic.ContainsKey(playerid);
    }
    public VerifyInfo Find(long playerid)
    {
        VerifyInfo info = null;
        verifyPlayerDic.TryGetValue(playerid, out info);
        return info;
    }
}

