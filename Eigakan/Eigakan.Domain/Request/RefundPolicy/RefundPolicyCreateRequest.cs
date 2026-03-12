using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.RefundPolicy
{
    public class RefundPolicyCreateRequest
    {
        [Required(ErrorMessage = "PolicyName is required.")]
        public string PolicyName { get; set; }

        [Required(ErrorMessage = "RefundPercent is required.")]
        [Range(0, 100, ErrorMessage = "RefundPercent phải nằm trong khoảng từ 0 đến 100.")]
        public int RefundPercent { get; set; }

        [Required(ErrorMessage = "Min is required.")]
        public int Min { get; set; }

        [Required(ErrorMessage = "Max is required.")]
        public int Max { get; set; }
    }
}
