using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Model
{
    public class CardData
    {
        [BsonElement("myrmidonCard")]
        public LinkedList<MyrmidonCard> myrmidonCards { get; set; }   //家将卡牌
    }
    /// <summary>
    /// 家将卡
    /// </summary>
    public class MyrmidonCard : Item 
    {
        [BsonElement("a")]
        public List<int> equips;       //装备
        [BsonElement("b")]
        public List<SkillInfo> skills;//key:技能id value;等级
        [BsonElement("c")]
        public byte star;               //升星
        [BsonElement("d")]
        public byte quality;            //品质
        [BsonElement("e")]
        public byte level;              //等级<=100
        [BsonElement("f")]
        public int exp;                //经验
    }
    /// <summary>
    /// 技能信息
    /// </summary>
    public class SkillInfo
    {
        [BsonElement("a")]
        public int skillid;
        [BsonElement("b")]
        public byte level;
    }
}
