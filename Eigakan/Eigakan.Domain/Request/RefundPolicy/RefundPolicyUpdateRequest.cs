using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.RefundPolicy
{
    public class RefundPolicyUpdateRequest
    {
        public string? PolicyName { get; set; }
        public string? RefundPercent { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
  
        public string? Status { get; set; }
    }
}
