using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdMedia
{
    public class AdMediaCreateRequest
    {
        public string? Content { get; set; }
        public string? Image { get; set; }
        public string? Video { get; set; }
        public string? Url { get; set; }
        public string? AdPurchaseSlotId { get; set; }
    }
}
