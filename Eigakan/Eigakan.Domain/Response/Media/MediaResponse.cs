using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.Media
{
    public class MediaResponse
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? MovieId { get; set; }
    }
}
