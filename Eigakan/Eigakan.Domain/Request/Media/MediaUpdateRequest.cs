using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.Media
{
    public class MediaUpdateRequest
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }

    }
}
