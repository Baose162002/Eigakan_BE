using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
    public class News
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Picture { get; set; }
        public string? Url { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
