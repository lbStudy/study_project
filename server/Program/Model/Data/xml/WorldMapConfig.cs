using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


namespace Config
{
    public class RoomNearConfig
    {
        public int roomid;
        public int from;
        public int to;
        public string condition;
    }
    public class RoomBaseConfig
    {
        public long id;
        public string cfname;
        public string name;
        public int serverid;
        public float pos_x;
        public float pos_y;
        //相连房间
        public Dictionary<long, RoomNearConfig> nearDic = new Dictionary<long, RoomNearConfig>();
    }
    public class CampInfo
    {
        public int id;
        public string name;
        public Dictionary<long, RoomBaseConfig> roomdic = new Dictionary<long, RoomBaseConfig>();
    }

    public class WorldMapConfig
    {
        public Dictionary<int, CampInfo> campDic = new Dictionary<int, CampInfo>();

        public void ParseXml(string content)
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(content);
            XmlNode root = xmlDoc.SelectSingleNode("map");

            // 阵营
            XmlNodeList nodeList = root.SelectNodes("camp");
            if (nodeList != null)
            {
                foreach(XmlNode node in nodeList)
                {
                    XmlElement campElem = node as XmlElement;
                    CampInfo campInfo = XmlHelper.GreateAndSetValue<CampInfo>(campElem);
                    campDic.Add(campInfo.id, campInfo);
                    //房间
                    XmlNodeList nodeList2 = node.SelectNodes("room");
                    if(nodeList2 != null)
                    {
                        foreach(XmlNode node2 in nodeList2)
                        {
                            XmlElement roomElem = node2 as XmlElement;
                            RoomBaseConfig roomInfo = XmlHelper.GreateAndSetValue<RoomBaseConfig>(roomElem);
                            campInfo.roomdic.Add(roomInfo.id, roomInfo);
                            //房间相连信息
                            XmlNodeList nodeList3 = node2.SelectNodes("near");
                            if (nodeList3 != null)
                            {
                                foreach (XmlNode node3 in nodeList3)
                                {
                                    XmlElement nearElem = node3 as XmlElement;
                                    RoomNearConfig nearInfo = XmlHelper.GreateAndSetValue<RoomNearConfig>(nearElem);
                                    roomInfo.nearDic.Add(nearInfo.roomid, nearInfo);
                                }
                            }
                        }
                    }
                }
            }
        }

        public RoomBaseConfig FindRoom(long roomid)
        {
            RoomBaseConfig roomBaseConfig = null;
            foreach (CampInfo campInfo in campDic.Values)
            {
                if(campInfo.roomdic.TryGetValue(roomid, out roomBaseConfig))
                {
                    return roomBaseConfig;
                }
            }
            return roomBaseConfig;
        }
        public RoomBaseConfig FindRoom(int campid, int roomid)
        {
            CampInfo campConfig = null;
            RoomBaseConfig roomConfig = null;
            if(campDic.TryGetValue(campid, out campConfig))
            {
                campConfig.roomdic.TryGetValue(roomid, out roomConfig);
            }
            return roomConfig;
        }
        public List<RoomBaseConfig> GetAllRoomInServer(int serverid)
        {
            List<RoomBaseConfig> roomBaseConfigs = new List<RoomBaseConfig>();
            foreach (CampInfo campInfo in campDic.Values)
            {
                foreach(RoomBaseConfig val in campInfo.roomdic.Values)
                {
                    if(val.serverid == serverid)
                    {
                        roomBaseConfigs.Add(val);
                    }
                }
            }

            return roomBaseConfigs;
        }
    }

}