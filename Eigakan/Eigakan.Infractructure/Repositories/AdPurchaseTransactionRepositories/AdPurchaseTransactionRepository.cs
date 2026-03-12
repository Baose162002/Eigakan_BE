using Discord;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.AdPurchaseTransactionRepositories
{
    public class AdPurchaseTransactionRepository : GenericBase<AdPurchaseTransaction>, IAdPurchaseTransactionRepository
    {
        public async Task<List<AdPurchaseTransaction>> GetAllAdPurchaseTransaction(int page, int pageSize)
        {
            var adtransaction = await Get(
                orderBy: c => c.OrderByDescending(u => u.CreateAt),
				includeProperties: "AdPurchaseItems",
				pageIndex: page,
                pageSize: pageSize);
            return adtransaction.ToList();
        }
        public async Task<AdPurchaseTransaction> GetAdPurchaseTransactionById(string id)
        {
            return await GetSingle(filter: c => c.Id == id);
        }
        public async Task<List<AdPurchaseTransaction>> GetAdPurchaseTransactionByUserId(string userId)
        {
            var adtransaction = await Get(filter: c => c.UserId == userId);
            return adtransaction.ToList();
        }

        public async Task<List<AdPurchaseTransaction>> GetAdPurchaseTransactionByUserIdPaging(string userId, int page, int pageSize)
        {
            var adtransaction = await Get(
				orderBy: c => c.OrderByDescending(u => u.CreateAt),
				filter: c => c.UserId == userId,
                includeProperties: "AdPurchaseItems",
                pageIndex: page,
                pageSize: pageSize);
            return adtransaction.ToList();
        }
        public async Task<int> CountAllAdPuchaseTransactionAsync()
        {
            return await CountAsync();
        }

        public async Task<int> CountAllAdPuchaseTransactionByUserIdAsync(string userId)
        {
            return await CountAsync(c => c.UserId == userId);
        }
    }
}
