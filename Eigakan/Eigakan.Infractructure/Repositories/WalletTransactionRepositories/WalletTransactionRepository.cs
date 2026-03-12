using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.WalletTransactionRepositories
{
    public class WalletTransactionRepository : GenericBase<Domain.Models.WalletTransaction>, IWalletTransactionRepository
    {
        public async Task<WalletTransaction?> GetWalletTransactionById(string walletId)
        {
            return await GetSingle(filter: c => c.Id == walletId);
        }
        public async Task<List<WalletTransaction>> GetPendingTransactionsBefore(DateTime thresholdTime)
        {
            var wallet = await Get(
                filter: t => t.Status == "PENDING" && t.CreateDate <= thresholdTime
            );
            return wallet.ToList();
        }
        public async Task<List<WalletTransaction>> GetWalletTransactionByUser(string userwalletId,int page, int pageSize)
        {
            var slotTime = await Get(
                orderBy: q => q.OrderByDescending(u => u.CreateDate),
                filter: q => q.UserWalletId == userwalletId,
                pageIndex: page,
                pageSize: pageSize
            );
            return slotTime.ToList();
        }
    }
}
