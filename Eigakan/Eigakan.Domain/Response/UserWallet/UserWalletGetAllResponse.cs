using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.UserWallet
{
    public class UserWalletGetAllResponse
    {
        public string? Id { get; set; }
        public decimal? Balance { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
    }
}
