using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.AdSlot
{
    public class AdSlotCreateRequest
    {
        public string SlotLocation { get; set; }
        public decimal SlotPrice { get; set; }

      
    }
}
