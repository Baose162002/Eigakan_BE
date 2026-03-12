using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.WalletTranasction;
using Eigakan.Domain.Response.WalletTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IWalletTransactionService
    {
        Task<Result<WalletTransaction>> CreatePayment(WalletTransactionCreateRequest request);
        Task<Result<WalletTransaction>> PaymentReturn(WalletTransactionStatus request);
        Task<Result<List<WalletTransactionGetAllResponse>>> GetListTransactionForCurrentUser(int page, int pageSize);
    }
}
