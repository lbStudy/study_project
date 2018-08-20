using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model
{
    public class BaseEntity
    {
        /// <summary>
        /// 基类对象的ID，MongoDB要求每个实体类必须有的主键
        /// </summary>
        [BsonRepresentation(BsonType.Int64)]
        public long id { get; set; }
    }
}