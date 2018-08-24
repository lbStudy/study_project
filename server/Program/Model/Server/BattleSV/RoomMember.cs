using System;
using System.Collections.Generic;
using Data;
using Base;
using System.IO;

public class RoomMember
{
    public RoomMemberInfo info;
    public int fromGateId;
    public string checkcode;
    public int maxScore;
    public long delay;
    public long clientTime;
    public int rank;
    public void Clear()
    {
    }
    public void SendMsgToGateserver(MemoryStream stream)
    {
        //Session gateSession = NetInnerComponent.Instance.GetByAppID(fromGateId);
        //if(gateSession == null || gateSession.IsDisposed)
        //{
        //    return;
        //}
        //gateSession.SendMessage(stream);
    }
    public void SendMsgToGateserver(object msg)
    {
        //Session gateSession = NetInnerComponent.Instance.GetByAppID(fromGateId);
        //if (gateSession == null || gateSession.IsDisposed)
        //{
        //    return;
        //}
        //gateSession.SendMessage(msg, info.playerid);
    }
}