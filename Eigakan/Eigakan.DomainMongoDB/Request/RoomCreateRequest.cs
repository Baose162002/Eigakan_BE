using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.DomainMongoDB.Request
{
    public class RoomCreateRequest
    {
        public string FileUrl { get; set; } // Link phim/video
        public DateTime CreateDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; } = true; // Khi hết phim sẽ set về false
        public string MovieID { get; set; }

    }
}
