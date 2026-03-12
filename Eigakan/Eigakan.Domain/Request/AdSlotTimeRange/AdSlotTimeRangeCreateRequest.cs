using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdSlotTimeRange
{
    public class AdSlotTimeRangeCreateRequest
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal SlotTimeRangePrice { get; set; }

    }
}
