using System;
using System.Collections.Generic;
using Base;
using Data;
using Config;

public class RoomManagerComponent : Component, IAwake
{
    public static RoomManagerComponent Instance;
    private Dictionary<long, Room> roomDic = new Dictionary<long, Room>();
    
    private List<long> emptyRooms = new List<long>();

    
    public void Awake()
    {
        Instance = this;
        CreateRoom();
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
    public void CreateRoom()
    {
        List<RoomBaseConfig> roomBaseConfigs = XmlDataComponent.Instance.worldMapConfig.GetAllRoomInServer(Game.Instance.Appid);
        foreach(RoomBaseConfig r in roomBaseConfigs)
        {
            RoomConfig roomConfig = XmlDataComponent.Instance.LoadRoomConfig(ref r.cfname);
            if(roomConfig != null)
            {
                Room room = new Room();
                room.Init(roomConfig, r);
                Add(room);
            }
            else
            {
                Console.WriteLine($"Not exist roomconfig : {r.cfname}");
            }
        }
    } 
    public Room Find(long roomid)
    {
        Room room = null;
        roomDic.TryGetValue(roomid, out room);
        return room;
    }
    public void Add(Room room)
    {
        roomDic[room.roomid] = room;
    }
    public void Remove(long roomid)
    {
        roomDic.Remove(roomid);
    }
    public void AddEmptyRoom(long roomid)
    {
        if (emptyRooms.Contains(roomid))
            return;
        emptyRooms.Add(roomid);
    }
    public void RemoveEmptyRoom(long roomid)
    {
        emptyRooms.Remove(roomid);
    }
    public void CloseRoom(long roomid)
    {
        Room room = null;
        if(roomDic.TryGetValue(roomid, out room))
        {
            SS_RemoveRoomMessage msgToM = new SS_RemoveRoomMessage();
            msgToM.id = roomid;
            msgToM.battleAppid = Game.Instance.Appid;
            Session managerSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.ManagerAppId);
            managerSession.SendMessage(msgToM, 0);

            Remove(roomid);
        }
    }
    public void CloseAllRoom()
    {
        List<Room> rooms = new List<Room>(roomDic.Values);
        foreach(Room room in rooms)
        {
            CloseRoom(room.roomid);
        }
        emptyRooms.Clear();
        roomDic.Clear();
    }
}
