using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdPurchaseSlot
{
    public class AdPurchaseSlotUpdateRequest
    {
        public string Content { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public string UrlLink { get; set; }



        public string Status { get; set; }

        public string AdSlotTimeID { get; set; }

        public string AdPurchaseID { get; set; }


        public string AdPackageID { get; set; }
    }
}
