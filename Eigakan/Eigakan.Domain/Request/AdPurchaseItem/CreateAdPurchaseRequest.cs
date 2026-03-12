using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdPurchaseItem
{
    public class NewAdMediaDto
    {
        public string? Content { get; set; }
        public string? Url { get; set; }
    }

    public class AdPurchaseItemRequest
    {
        public int ViewQuantity { get; set; }
        public string? MediaId { get; set; }  // optional
        public NewAdMediaDto? NewMedia { get; set; }  // optional
    }

    public class CreateAdPurchaseRequest
    {
        public List<AdPurchaseItemRequest> AdPurchaseItems { get; set; }
    }

}
