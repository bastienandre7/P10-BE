using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PatientNoteApi.Models
{
    public class PatientNote
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string patId { get; set; }
        public string Patient { get; set; } = null!;
        public string Note { get; set; } = null!;
    }
}
