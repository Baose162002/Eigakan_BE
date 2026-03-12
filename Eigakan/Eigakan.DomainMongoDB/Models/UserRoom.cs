using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.DomainMongoDB.Models
{
    public class UserRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoomId { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsHost { get; set; }
    }

}
