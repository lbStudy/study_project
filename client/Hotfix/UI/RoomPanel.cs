using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Config;
using System;

namespace Hotfix
{
    public class RoomPanel : Frame
    {
        public GameObject npc_prefab;
        public GameObject line_prefab;
        public GameObject room_prefab;
        public Transform content;
        public Transform npc_parent;

        float height;
        float width;

        List<Transform> lines = new List<Transform>();
        List<Transform> rooms = new List<Transform>();
        List<Transform> npcGos = new List<Transform>();

        Vector2 offset = new Vector2(60, 50);

        int lineCount;
        int roomCount;
        int npcCount;

        public override void Init()
        {
            base.Init();
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            npc_prefab = rc.Get<GameObject>("npc");
            line_prefab = rc.Get<GameObject>("line");
            room_prefab = rc.Get<GameObject>("room_btn");
            content = rc.Get<GameObject>("content").transform;
            npc_parent = rc.Get<GameObject>("npcs").transform;
            Button worldmap_btn = rc.Get<GameObject>("worldmap_btn").GetComponent<Button>();
            worldmap_btn.onClick.AddListener(() =>
            {
                UIManager.Instance.Show(FrameType.WorldMapPanel, null, ShowLayer.Layer_4);
            });
            npc_prefab.SetActive(false);
            line_prefab.SetActive(false);
            room_prefab.SetActive(false);

            RectTransform rt_room = room_prefab.transform as RectTransform;
            width = rt_room.rect.width;
            height = rt_room.rect.height;
        }
        public override void Destroy()
        {
            base.Destroy();
        }
        public override void Show(object[] arg)
        {
            base.Show(arg);
            Refresh();
        }

        public void Refresh()
        {
            //Room room = Game.Instance.room;
            //Player localPlayer = Game.Instance.LocalPlayer;
            //lineCount = 0;
            //roomCount = 0;
            //npcCount = 0;
            //if (room != null)
            //{
            //    //局部地图
            //    RoomBaseConfig baseConfig = ConfigDataManager.Instance.worldMapConfog.FindRoom((int)localPlayer.roomId);
            //    if(baseConfig != null)
            //    {
            //        Transform room_go = GetRoomGo();
            //        RefreshRoomBtn(baseConfig, null, room_go);
            //        roomCount++;
            //        foreach(RoomNearConfig near in baseConfig.nearDic.Values)
            //        {
            //            RoomBaseConfig otherConfig = ConfigDataManager.Instance.worldMapConfog.FindRoom(near.roomid);
            //            if(otherConfig != null)
            //            {
            //                Transform to_room_go = GetRoomGo();
            //                RefreshRoomBtn(otherConfig, near, to_room_go);
            //                roomCount++;

            //                RefreshLine(room_go.localPosition, to_room_go.localPosition);
            //                lineCount++;
            //            }
            //        }
            //    }
            //    //房间npc
            //    RoomConfig roomConfig = ConfigDataManager.Instance.LoadRoomXml((int)localPlayer.roomId);
            //    if(roomConfig != null)
            //    {
            //        foreach(NpcConfig npccf in roomConfig.npcDic.Values)
            //        {

            //            Transform npc_go = GetNpcGo();
            //            RefreshNpc(npccf, npc_go);
            //            npcCount++;
            //        }
            //    }
            //}
            //for (int i = roomCount; i < rooms.Count; i++)
            //{
            //    if (rooms[i].gameObject.activeSelf)
            //        rooms[i].gameObject.SetActive(false);
            //}
            //for (int i = lineCount; i < lines.Count; i++)
            //{
            //    if (lines[i].gameObject.activeSelf)
            //        lines[i].gameObject.SetActive(false);
            //}
            //for (int i = npcCount; i < npcGos.Count; i++)
            //{
            //    if (npcGos[i].gameObject.activeSelf)
            //        npcGos[i].gameObject.SetActive(false);
            //}
        }
        Transform GetRoomGo()
        {
            Transform tRoom = null;
            if (roomCount < rooms.Count)
            {
                tRoom = rooms[roomCount];
            }
            else
            {
                tRoom = GameObject.Instantiate<GameObject>(room_prefab, content).transform;
                rooms.Add(tRoom);
            }
            if (tRoom.gameObject.activeSelf == false)
                tRoom.gameObject.SetActive(true);
            return tRoom;
        }
        //Transform GetNpcGo()
        //{
        //    Transform npcGO = null;
        //    if (npcCount < npcGos.Count)
        //    {
        //        npcGO = npcGos[npcCount];
        //    }
        //    else
        //    {
        //        npcGO = GameObject.Instantiate<GameObject>(npc_prefab, npc_parent).transform;
        //        npcGos.Add(npcGO);
        //    }
        //    if (npcGO.gameObject.activeSelf == false)
        //        npcGO.gameObject.SetActive(true);
        //    return npcGO;
        //}
        //void RefreshNpc(NpcConfig npccf, Transform npc_go)
        //{
        //    Button btn = npc_go.GetComponent<Button>();
        //    btn.onClick.RemoveAllListeners();
        //    int npcid = npccf.id;
        //    btn.onClick.AddListener(() =>
        //    {
        //        MeetNpc(npcid);
        //    });
        //    npc_go.GetComponentInChildren<Text>().text = npccf.id.ToString();
        //}
        //void RefreshRoomBtn(RoomBaseConfig baseConfig, RoomNearConfig near, Transform room_go)
        //{
        //    Button btn = room_go.GetComponent<Button>();
        //    btn.onClick.RemoveAllListeners();
        //    Vector3 pos = Vector3.zero;
        //    if (near != null)
        //    {
        //        int roomid = baseConfig.id;
        //        btn.onClick.AddListener(() =>
        //        {
        //            EnterRoom(roomid);
        //        });

        //        if(near.from == 1)
        //        {//上
        //            pos = new Vector3(0, height + offset.y, 0);
        //        }
        //        else if(near.from == 2)
        //        {//左
        //            pos = new Vector3(width + offset.x, 0, 0);
        //        }
        //        else if (near.from == 3)
        //        {//下
        //            pos = new Vector3(0, -(height + offset.y), 0);
        //        }
        //        else if (near.from == 4)
        //        {//右
        //            pos = new Vector3(-(width + offset.x), 0, 0);
        //        }
        //    }
        //    room_go.localPosition = pos;
        //    room_go.GetComponentInChildren<Text>().text = baseConfig.name;
        //}
        //void RefreshLine(Vector3 startPos, Vector3 endPos)
        //{
        //    Transform line = null;
        //    if (lineCount < lines.Count)
        //    {
        //        line = lines[lineCount];
        //    }
        //    else
        //    {
        //        line = GameObject.Instantiate<GameObject>(line_prefab, content).transform;
        //        lines.Add(line);
        //    }
        //    if (line.gameObject.activeSelf == false)
        //        line.gameObject.SetActive(true);
        //    line.transform.localPosition = (startPos + endPos) * 0.5f;
        //    line.localScale = new Vector3(30, 1, 1);
        //    Vector3 dir = (endPos - startPos).normalized;
        //    float angle = Vector3.Angle(Vector3.right, dir);
        //    if (dir.y < 0)
        //    {
        //        angle = -angle;
        //    }
        //    line.localEulerAngles = new Vector3(0, 0, angle);
        //}
        //async void EnterRoom(int roomid)
        //{
        //    Debug.Log($"onclick room {roomid}");
        //    bool isSuccess = await Game.Instance.LocalPlayer.EnterRoom(roomid);
        //    GlobalEventManager.Trigger(GlobalEventType.AddLeftBottomDes, new EventPackage(0, 0, $"欢迎进入房间{roomid}!"));
        //}
        //void MeetNpc(int npcid)
        //{
        //    Debug.Log($"onclick npc {npcid}");
        //}
        //public void RoomInfoChangeHandle(EventPackage package)
        //{
        //    if (State != FrameState.Show)
        //        return;
        //    Refresh();
        //}

    }
}

