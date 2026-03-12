using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdSlotTime
{
    public class AdSlotTimeUpdateRequest
    {
        public decimal SlotTimePrice { get; set; }
        public string AdSlotTimeRangeID { get; set; }
        public string AdSlotID { get; set; }

    }
}
