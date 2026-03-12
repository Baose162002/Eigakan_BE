using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdMedia
{
    public class AdClickCountCreateRequest
    {
        public string? AdMediaId { get; set; }
        public string? MovieId { get; set; }
    }
} 