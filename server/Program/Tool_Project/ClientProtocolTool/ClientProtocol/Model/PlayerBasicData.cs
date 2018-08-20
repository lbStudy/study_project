using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model
{
    public class PlayerBasicData
    {
        [BsonElement("a")]
        public long playerid { get; set; }
        [BsonElement("b")]
        public string name { get; set; }                //名字
        [BsonElement("c")]
        public short iconID { get; set; }              //头像
        [BsonElement("d")]
        public byte level { get; set; }                 //等级<=100
        [BsonElement("f")]
        public byte sex { get; set; }                   //性别<100
        [BsonElement("g")]
        public byte vip { get; set; }                   //VIP
        [BsonElement("h")]
        public long unionID;                            //工会

    }
    public class ScenePlayerInfo : PlayerBasicData
    {
        public int x;
        public int y;
        public int z;
        public int sceneid;
    }
}
