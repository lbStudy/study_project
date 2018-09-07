using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Base
{
    public class DBComponent : Component
    {
       [BsonRepresentation(BsonType.Int64)]
        public long id;
        [BsonIgnore]
        public bool isSave;
        [BsonIgnore]
        ReplaceOneModel<DBComponent> replaceOneModel;


        public ReplaceOneModel<DBComponent> GetReplaceOneModel()
        {
            if (replaceOneModel == null)
            {
                var filter = Builders<DBComponent>.Filter.Eq(f => f.id, this.id);
                replaceOneModel = new ReplaceOneModel<DBComponent>(filter, this);
                replaceOneModel.IsUpsert = true;
            }
            return replaceOneModel;
        }
        public virtual void FinishSaveHandle()
        {
            isSave = false;
        }
    }
}
