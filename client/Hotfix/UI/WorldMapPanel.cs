using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using Config;

namespace Hotfix
{
    public class WorldMapPanel : Frame
    {
        Transform content;
        GameObject line_prefab;
        GameObject room_prefab;

        List<Transform> lines = new List<Transform>();
        List<Text> txt_rooms = new List<Text>();

        float halfofheight;
        float halfofwidth;
        int connectCount = 0;

        Rect dragRange = new Rect();
        public override void Init()
        {
            base.Init();

            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            content = rc.Get<GameObject>("content").transform;
            line_prefab = rc.Get<GameObject>("line");
            room_prefab = rc.Get<GameObject>("txt_room");
            line_prefab.SetActive(false);
            room_prefab.SetActive(false);
            lines.Add(line_prefab.transform);
            txt_rooms.Add(room_prefab.GetComponent<Text>());
            RectTransform rt_room = room_prefab.transform as RectTransform;
            halfofwidth = rt_room.rect.width / 2;
            halfofheight = rt_room.rect.height / 2;

            GameObject dragArea = rc.Get<GameObject>("dragArea");
            dragArea.GetComponent<UIListenEvent>().drag = DragHandle;

            Button exit_btn = rc.Get<GameObject>("exit_btn").GetComponent<Button>();
            exit_btn.onClick.AddListener(() =>
            {
                Hide();
            });
        }

        public override void Show(object[] arg)
        {
            base.Show(arg);
            Refresh();
        }
        void DragHandle(PointerEventData eventData)
        {
            Vector2 dir = eventData.delta.normalized;
            Vector2 curP = new Vector2(content.localPosition.x, content.localPosition.y);
            Vector2 endP = dir * 20 + curP;
            if (endP.x > dragRange.xMax)
            {
                endP.x = dragRange.xMax;
            }
            else if (endP.x < dragRange.xMin)
            {
                endP.x = dragRange.xMin;
            }
            if (endP.y > dragRange.yMax)
            {
                endP.y = dragRange.yMax;
            }
            else if (endP.y < dragRange.yMin)
            {
                endP.y = dragRange.yMin;
            }
            content.localPosition = new Vector3(endP.x, endP.y, 0);
        }
        public void Refresh()
        {
            //if (ConfigDataManager.Instance.worldMapConfog != null)
            //{
            //    //显示地图房间
            //    int roomCount = 0;
            //    float halfofscreenwidth = Screen.width * 0.5f;
            //    float halfofscreenheight = Screen.height * 0.5f;
            //    foreach (CampInfo cmapInfo in ConfigDataManager.Instance.worldMapConfog.campDic.Values)
            //    {
            //        foreach(RoomBaseConfig roomInfo in cmapInfo.roomdic.Values)
            //        {
            //            Text tRoom = null;
            //            if (roomCount < txt_rooms.Count)
            //            {
            //                tRoom = txt_rooms[roomCount];
            //            }
            //            else
            //            {
            //                tRoom = GameObject.Instantiate<GameObject>(room_prefab, content).GetComponent<Text>();
            //                txt_rooms.Add(tRoom);
            //            }
            //            if (tRoom.gameObject.activeSelf == false)
            //                tRoom.gameObject.SetActive(true);
            //            tRoom.transform.localPosition = new Vector3(roomInfo.pos_x, roomInfo.pos_y, 0);
            //            if(roomInfo.pos_x - halfofscreenwidth > dragRange.xMax - halfofwidth)
            //            {
            //                dragRange.xMax = roomInfo.pos_x - halfofscreenwidth + halfofwidth;
            //            }
            //            if (roomInfo.pos_x + halfofscreenwidth < dragRange.xMin + halfofwidth)
            //            {
            //                dragRange.xMin = roomInfo.pos_x + halfofscreenwidth - halfofwidth;
            //            }
            //            if (roomInfo.pos_y - halfofscreenheight > dragRange.yMax - halfofheight)
            //            {
            //                dragRange.yMax = roomInfo.pos_y - halfofscreenheight + halfofheight;
            //            }
            //            if (roomInfo.pos_y + halfofscreenheight < dragRange.yMin + halfofheight)
            //            {
            //                dragRange.yMin = roomInfo.pos_y + halfofscreenheight - halfofheight;
            //            }
            //            tRoom.text = roomInfo.name;
            //            roomCount++;
            //        }
            //    }
            //    //相反，例如：向上滑动其实是看下方的内容，向右滑动是看左方内容
            //    float xmax = dragRange.xMax;
            //    dragRange.xMax = -dragRange.xMin;
            //    dragRange.xMin = -xmax;
            //    float ymax = dragRange.yMax;
            //    dragRange.yMax = -dragRange.yMin;
            //    dragRange.yMin = -ymax;
            //    for (int i = roomCount; i < txt_rooms.Count; i++)
            //    {
            //        if(txt_rooms[i].gameObject.activeSelf)
            //            txt_rooms[i].gameObject.SetActive(false);
            //    }
            //    //显示房间连接
            //    List<int> connects = new List<int>();
            //    connectCount = 0;
            //    foreach (CampInfo cmapInfo in ConfigDataManager.Instance.worldMapConfog.campDic.Values)
            //    {
            //        foreach (RoomBaseConfig roomInfo in cmapInfo.roomdic.Values)
            //        {
            //            foreach(RoomNearConfig nearInfo in roomInfo.nearDic.Values)
            //            {
            //                if (connects.Contains(nearInfo.roomid))
            //                    continue;
            //                RoomBaseConfig nearRoomInfo = ConfigDataManager.Instance.worldMapConfog.FindRoom(nearInfo.roomid);

            //                RoomNearConfig to = null;
            //                nearRoomInfo.nearDic.TryGetValue(roomInfo.id, out to);
            //                Vector3 startPos = Vector3.zero;
            //                Vector3 endPos = Vector3.zero;
            //                if(to == null)
            //                {//存在单项通道
            //                    startPos = CalPos(roomInfo, nearInfo.from);
            //                    endPos = CalPos(nearRoomInfo, nearInfo.to);
            //                    RoomConnect(startPos, endPos, 1);
            //                }
            //                else if(nearInfo.from == to.to && nearInfo.to == to.from)
            //                {//双向通道
            //                    startPos = CalPos(roomInfo, nearInfo.from);
            //                    endPos = CalPos(nearRoomInfo, nearInfo.to);
            //                    RoomConnect(startPos, endPos, 2);
            //                }
            //                else
            //                {//两条通道
            //                    startPos = CalPos(roomInfo, nearInfo.from);
            //                    endPos = CalPos(nearRoomInfo, nearInfo.to);
            //                    RoomConnect(startPos, endPos, 1);

            //                    startPos = CalPos(nearRoomInfo, to.from);
            //                    endPos = CalPos(roomInfo, to.to);
            //                    RoomConnect(startPos, endPos, 1);
            //                }
            //            }
            //            connects.Add(roomInfo.id);
            //        }
            //    }
            //    for (int i = connectCount; i < lines.Count; i++)
            //    {
            //        if (lines[i].gameObject.activeSelf)
            //            lines[i].gameObject.SetActive(false);
            //    }
            //}
        }
        //public Vector3 CalPos(RoomBaseConfig roomInfo, int dir)
        //{
        //    if(dir == 1)
        //    {//上
        //        return new Vector3(roomInfo.pos_x, roomInfo.pos_y + halfofheight, 0);
        //    }
        //    else if(dir == 2)
        //    {//右
        //        return new Vector3(roomInfo.pos_x + halfofwidth, roomInfo.pos_y , 0);
        //    }
        //    else if (dir == 3)
        //    {//下
        //        return new Vector3(roomInfo.pos_x , roomInfo.pos_y - halfofheight, 0);
        //    }
        //    else if(dir == 4)
        //    {//左
        //        return new Vector3(roomInfo.pos_x - halfofwidth, roomInfo.pos_y, 0);
        //    }

        //    return Vector3.zero;
        //}
        public void RoomConnect(Vector3 startPos, Vector3 endPos, int connectType)
        {
            Transform line = null;
            if (connectCount < lines.Count)
            {
                line = lines[connectCount];
            }
            else
            {
                line = GameObject.Instantiate<GameObject>(line_prefab, content).transform;
                lines.Add(line);
            }
            if (line.gameObject.activeSelf == false)
                line.gameObject.SetActive(true);
            line.transform.localPosition = (startPos + endPos) * 0.5f;
            float dis = Vector3.Distance(startPos, endPos);
            line.localScale = new Vector3(dis, 1, 1);
            Vector3 dir = (endPos - startPos).normalized;
            float angle = Vector3.Angle(Vector3.right, dir);
            if (dir.y < 0)
            {
                angle = -angle;
            }
            line.localEulerAngles = new Vector3(0, 0, angle);
            connectCount++;
        }
    }
}

