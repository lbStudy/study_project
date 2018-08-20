using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model
{
    public class PlayerDetailData
    {
        public long playerid { get; set; }
        public string name { get; set; }
    }
}
