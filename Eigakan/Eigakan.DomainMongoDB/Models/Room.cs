using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.DomainMongoDB.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] 
        public string Id { get; set; }

        public string HostId { get; set; }  // Người tạo phòng (UserId)
        public string FileUrl { get; set; } // Link phim/video
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; } = true; // Khi hết phim sẽ set về false
        public string Status { get; set; }  // "Active" | "Ended"
        public string MovieID { get; set; }
    }
}
