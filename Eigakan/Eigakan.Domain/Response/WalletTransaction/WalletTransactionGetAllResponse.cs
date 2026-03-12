using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.WalletTransaction
{
    public class WalletTransactionGetAllResponse
    {
            public string? Id { get; set; }
            public decimal? Amount { get; set; }
            public string? Type { get; set; }
            public string? PaymentReferenceId { get; set; }
            public string? PaymentMethod { get; set; }
            public string? Status { get; set; }
            public DateTime? CreateDate { get; set; }
            public string? UserWalletId { get; set; }
    }
}
