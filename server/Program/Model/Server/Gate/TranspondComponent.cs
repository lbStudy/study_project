using System.Collections.Generic;
using Base;
using Data;
using System.IO;
using System;

public class TranspondInfo
{
    public long playerid;
    public int battleSvAppid;
    public Session clientSession;
}
public class TranspondComponent : Component, IAwake
{
    public static TranspondComponent instance;

    private Dictionary<long, TranspondInfo> infoDic = new Dictionary<long, TranspondInfo>();
    List<TranspondInfo> removeInfos = new List<TranspondInfo>();

    public void Awake()
    {
        instance = this;
        TimeTask timeTask = new IntervalTask(10000, TimeDetection, TimeTaskPriority.Middle);
        TimeManagerComponent.Instance.Add(timeTask);
    }
    public override void Dispose()
    {
        if (this.IsDisposed)
        {
            return;
        }

        base.Dispose();

        instance = null;
    }
    public void TimeDetection()
    {
        foreach (TranspondInfo info in infoDic.Values)
        {
            if (info.clientSession == null || Game.Instance.Msec - info.clientSession.pingTime > ConstConfigComponent.ConstConfig.PingTime)
            {
                removeInfos.Add(info);
            }
        }

        if (removeInfos.Count > 0)
        {
            for(int i = 0; i < removeInfos.Count; i++)
            {
                if(removeInfos[i].clientSession != null)
                    removeInfos[i].clientSession.Dispose();
            }
            removeInfos.Clear();
        }
    }
    public void Add(long playerid, Session clientSession)
    {
        TranspondInfo info = null;
        if (infoDic.TryGetValue(playerid, out info))
        {
            if (null != infoDic[playerid].clientSession)
            {
                infoDic[playerid].clientSession.relevanceID = 0;
                infoDic[playerid].clientSession.Dispose();
            }
        }
        else
        {
            info = new TranspondInfo();
            infoDic.Add(playerid, info);
        }
        info.playerid = playerid;
        info.clientSession = clientSession;
        clientSession.relevanceID = playerid;
        info.clientSession.pingTime = Game.Instance.Msec;
    }
    public TranspondInfo Find(long playerid)
    {
        TranspondInfo info = null;
        infoDic.TryGetValue(playerid, out info);
        return info;
    }
    public void SetBattleAppid(long playerid, int battleAppid)
    {
        TranspondInfo info = null;
        if (infoDic.TryGetValue(playerid, out info))
            info.battleSvAppid = battleAppid;
    }
    public ErrorCode ToServer(AppType targetApp, long id, MemoryStream stream)
    {
        TranspondInfo info = null;

        if(infoDic.TryGetValue(id, out info))
        {
            Session session = null;
            //if (targetApp == AppType.GameServer)
            //{
            //    session = NetInnerComponent.Instance.GetByAppID(info.gameSvAppid);
            //}
            //else if (targetApp == AppType.SceneServer)
            //{
            //    session = NetInnerComponent.Instance.GetByAppID(info.sceneSvAppid);
            //}
            //if (targetApp == AppType.WorldServer)
            //{
            //    session = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.WorldAppId);
            //}
            //if (targetApp == AppType.BattleServer)
            //{
            //    if(info.battleSvAppid > 0)
            //    {
            //        session = NetInnerComponent.Instance.GetByAppID(info.battleSvAppid);
            //    }
            //    else
            //    {
            //        //Log.Warning($"Warning: player({id}) not in battle, so not handle({msg.GetType().Name}).");
            //        return ErrorCode.NotInBattle;
            //    }
            //}
            //else if (targetApp == AppType.ChatServer)
            //{
            //    session = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.ChatAppId);
            //}
            if (session == null || session.IsDisposed)
            {//内网连接出现问题
                Log.Warning($"Warning: inner({targetApp}) session disconnect or is null in gateserver, so transpond  fail and disconnect player.");
                return ErrorCode.Fail;
            }
            session.SendMessage(stream);

            return ErrorCode.Success;
        }
        else
        {
            Log.Warning($"player({id}) not login, but want to transpond massage. so disconnect player.");
            return ErrorCode.Fail;
        }
    }
    public void ToClient(long id, MemoryStream stream)
    {
        TranspondInfo info = null;

        if (infoDic.TryGetValue(id, out info) && info.clientSession.IsDisposed == false)
        {
            info.clientSession.SendMessage(stream);
        }
        //else
        //{
        //    Log.Debug($"player leave, but want to transpond to massage({msg.GetType().Name}).");
        //}
    }
    public void ToClient(object msg, long id)
    {
        TranspondInfo info = null;

        if (infoDic.TryGetValue(id, out info) && info.clientSession.IsDisposed == false)
        {
            info.clientSession.SendMessage(msg, id);
        }
    }
    public void ClientDisconnect(long id)
    {
        TranspondInfo info = null;
        if(infoDic.TryGetValue(id, out info))
        {
            infoDic.Remove(id);
        }
    }
}