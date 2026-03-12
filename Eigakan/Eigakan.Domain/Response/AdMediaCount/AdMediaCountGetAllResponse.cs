using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.AdMediaCount
{
    public class AdMediaCountGetAllResponse
    {
        public string? Id { get; set; }
        public DateOnly? ViewDate { get; set; }
        public int? ViewCount { get; set; }
        public string? AdMediaId { get; set; }
    }
}
