using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Data
{
    /// <summary>
    /// 玩家数据，单独建表
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class PlayerData
    {
        /// <summary>
        /// 基类对象的ID，MongoDB要求每个实体类必须有的主键
        /// </summary>
        [BsonRepresentation(BsonType.Int64)]
        [ProtoBuf.ProtoMember(1)]
        public long id { get; set; }
        [BsonElement("detail")]
        [ProtoBuf.ProtoMember(2)]
        public PlayerDetailData detailData { get; set; }            //详细数据
        [BsonElement("extra")]
        [ProtoBuf.ProtoMember(3)]
        public PlayerExtraData extraData { get; set; }              //额外数据
    }
}






