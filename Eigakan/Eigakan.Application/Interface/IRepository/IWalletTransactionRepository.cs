using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IWalletTransactionRepository : IGenericRepository<WalletTransaction>
    {
        Task<WalletTransaction?> GetWalletTransactionById(string walletId);
        Task<List<WalletTransaction>> GetPendingTransactionsBefore(DateTime thresholdTime);
        Task<List<WalletTransaction>> GetWalletTransactionByUser(string userwalletId, int page, int pageSize);
    }
}
