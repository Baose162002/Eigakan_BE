using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.RefundPolicy
{
    public class RefundPolicyChangeStatusRequest
    {
        public string? RefundPolicyID { get; set; }
        public int Status { get; set; }
    }
}
