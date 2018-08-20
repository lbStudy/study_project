using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model
{
    public class PlayerTranscript
    {
        [BsonElement("a")]
        public int maxCommonID;                                 //当前闯过普通副本最大
        [BsonElement("b")]
        public int maxEliteID;                                  //当前闯过经验最大
        [BsonElement("c")]
        public List<TranscriptData> commonTranscript;//一般副本
        [BsonElement("d")]
        public List<TranscriptData> eliteTranscript; //精英副本
    }
    public class TranscriptData
    {
        [BsonElement("a")]
        public List<byte> stars;                        //获得闯过关卡获得星级
        [BsonElement("b")]
        public byte maxMission;                         //当前闯过副本最大关卡
        [BsonElement("c")]
        public byte getAwardLevel;                      //以获得奖励级别
    }
}
