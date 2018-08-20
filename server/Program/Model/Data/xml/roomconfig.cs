using Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Config
{

    /// <summary>
    /// npc
    /// </summary>
    public class NpcConfig
    {
        public int id;
        public int campid;
    }

    public class RoomConfig
    {
        public int id;
        public string name;
        public string des;
        public RoomType type;
        public Dictionary<int, NpcConfig> npcDic = new Dictionary<int, NpcConfig>();//房间NPC

        public void ParseXml(ref string content)
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(content);
            XmlNode root = xmlDoc.SelectSingleNode("room");

            id = Convert.ToInt32(root.Attributes["id"].Value);
            name = root.Attributes["name"].Value;
            des = root.Attributes["des"].Value;
            string roomTypeStr = root.Attributes["type"].Value;
            type = RoomType.room;
            if (Enum.TryParse(roomTypeStr, out type) == false)
            {
                Console.WriteLine($"roomconfig error : roomid:{id} roomtype:{roomTypeStr}");
            }

            // npc//
            XmlNodeList npcNodeList = root.SelectNodes("npc");
            if (npcNodeList != null)
            {
                foreach (XmlNode node in npcNodeList)
                {
                    XmlElement npcElem = node as XmlElement;
                    NpcConfig npcConfig = XmlHelper.GreateAndSetValue<NpcConfig>(npcElem);
                    npcDic.Add(npcConfig.id, npcConfig);
                }
            }
        }
    }
}
