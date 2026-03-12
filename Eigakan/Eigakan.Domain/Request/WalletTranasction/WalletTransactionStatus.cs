using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.WalletTranasction
{
    public class WalletTransactionStatus
    {
        public string? WalletTransactionID { get; set; }
        public string? vnp_TransactionNo { get; set; }
        public string? Status { get; set; }
    }
}
