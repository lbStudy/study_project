using System;
using System.Collections.Generic;
using Data;
using Base;
using Config;
using System.IO;

public class Room
{
    public long roomid;
    public RoomSynchInfo roomInfo;
    //待进入玩家
    private Dictionary<long, RoomMember> waitEnterMemberDic = new Dictionary<long, RoomMember>();
    //玩家
    private Dictionary<long, RoomMember> memberDic = new Dictionary<long, RoomMember>();
    private List<RoomMember> members = new List<RoomMember>();
    
    IntervalTask updateTask;

    Random random = new Random();

    public long preUpdateTime;
    float fixedTime;
    long ft;
    public float waitTime;
    public RoomState state { get { return (RoomState)roomInfo.state; } set { roomInfo.state = (int)value; } }

    public RoomConfig config;
    public RoomBaseConfig baseConfig;

    public void Init(RoomConfig config, RoomBaseConfig baseConfig)
    {
        this.roomid = config.id;
        this.config = config;
        this.baseConfig = baseConfig;
        roomInfo = new RoomSynchInfo();
        
        updateTask = new IntervalTask((int)(ConstDefine.fixedTime * 1000), Update);
        TimeManagerComponent.Instance.Add(updateTask);

        state = RoomState.Idle;//等待开始
    }
    public void Close()
    {
        TimeManagerComponent.Instance.Remove(updateTask.Id);

        foreach(RoomMember member in members)
        {
            member.Clear();
        }
    }
    void Update()
    {
        if (preUpdateTime == 0)
        {
            preUpdateTime = TimeHelper.ClientNow();
            return;
        }
        else
        {
            ft = TimeHelper.ClientNow() - preUpdateTime;
            if(ft < 0)
            {
                ft = -ft;
            }
            fixedTime = ft * ConstDefine.shrink;
            preUpdateTime = TimeHelper.ClientNow();
        }
        if (state == RoomState.Idle)
        {
            waitTime += fixedTime;
        }
        else if (state == RoomState.Runing)
        {
            roomInfo.surplusTime -= ft;
            if (roomInfo.surplusTime <= 0)
            {
                //End();
                return;
            }
            
        }
    }
    void CreateRoomRobot(int count)
    {
        for(int i = 0; i < count; i++)
        {
            RoomMember member = new RoomMember();
            member.fromGateId = 0;
            member.checkcode = string.Empty;
            member.info = new RoomMemberInfo();
            member.info.playerid = IdGenerater.GenerateId();
            member.info.isCreator = false;
            member.info.name = "robot" + i;
            member.info.enterTime = Game.Instance.Msec;
            member.info.isOnline = false;
            AddMember(member);
            //CreateMemberCharacter(member);
        }
    }

    #region 通用
    public int RandomVal(int min, int max)
    {
        return random.Next(min, max);
    }
    #endregion
    #region 成员操作
    public void AddWaitEnterMember(RoomMember member)
    {
        waitEnterMemberDic[member.info.playerid] = member;
    }
    public void RemoveWaitEnterMember(RoomMember member)
    {
        waitEnterMemberDic.Remove(member.info.playerid);
    }
    public RoomMember FindWaitEnterMember(long playerid)
    {
        RoomMember member = null;
        waitEnterMemberDic.TryGetValue(playerid, out member);
        return member;
    }

    public void AddMember(RoomMember member)
    {
        if (memberDic.ContainsKey(member.info.playerid))
            return;
        memberDic[member.info.playerid] = member;
        members.Add(member);
    }
    public bool IsExistMember(long playerid)
    {
        return memberDic.ContainsKey(playerid);
    }
    public void RemoveMember(RoomMember member)
    {
        memberDic.Remove(member.info.playerid);
        members.Remove(member);
    }
    public RoomMember Find(long playerid)
    {
        RoomMember member = null;
        memberDic.TryGetValue(playerid, out member);
        return member;
    }
    #endregion
    #region 广播
    public void BroadCastChat(object msg, bool isFore = false)
    {

    }
    public void BroadcastMsg(object msg, long noid = 0, bool isForce = false)
    {
        Packet packet = Packet.Take();
        MemoryStream stream = packet.Stream;
        try
        {
            Session.FillContent(stream, msg, 0, 0);
            foreach (RoomMember val in memberDic.Values)
            {
                if (val.info.playerid == noid)
                    continue;
                if (val.info.isOnline || isForce)
                {
                    Session.ReplaceToid(stream, val.info.playerid);
                    val.SendMsgToGateserver(stream);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
        finally
        {
            Packet.Back(packet);
        }
    }
    
    #endregion
    #region 获得信息
    public void GetMemberInfos(List<RoomMemberInfo> memberInfos, long notId)
    {
        foreach (RoomMember val in members)
        {
            if (val.info.playerid == notId)
            {
                continue;
            }
            memberInfos.Add(val.info);
        }
    }
    #endregion

}
