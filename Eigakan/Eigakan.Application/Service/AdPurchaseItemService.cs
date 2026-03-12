using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.AdPurchaseItem;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class AdPurchaseItemService : IAdPurchaseItemService
    {
        private readonly IAdPurchaseTransactionRepository _adPurchaseTransactionRepository;
        private readonly IAdPurchaseItemRepository _adPurchaseItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AdPurchaseItemService(
            IAdPurchaseTransactionRepository adPurchaseTransactionRepository,
            IAdPurchaseItemRepository adPurchaseItemRepository,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _adPurchaseTransactionRepository = adPurchaseTransactionRepository;
            _adPurchaseItemRepository = adPurchaseItemRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<(List<AdPurchaseItemGetAllResponse> Items, int Total)> GetUserAdPurchaseHistoryAsync(int page, int pageSize)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return (new List<AdPurchaseItemGetAllResponse>(), 0);
            }

            var transactions = await _adPurchaseTransactionRepository.Get(
                filter: t => t.UserId == userId,
                includeProperties: "AdPurchaseItems.AdPackage,AdPurchaseItems.AdMedia,User"
            );

            if (transactions == null || !transactions.Any())
            {
                return (new List<AdPurchaseItemGetAllResponse>(), 0);
            }

            var allItems = transactions
                .Where(t => t.AdPurchaseItems != null)
                .SelectMany(t => t.AdPurchaseItems!)
                .OrderByDescending(i => i.CreatedDate)
                .ToList();

            var total = allItems.Count;

            var pagedItems = allItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedItems = _mapper.Map<List<AdPurchaseItemGetAllResponse>>(pagedItems);

            return (mappedItems, total);
        }

        public async Task<(List<AdPurchaseItemGetAllResponse> Items, int Total, decimal? totalConsumed, decimal? totalPurchased)> GetAllAdPurchaseHistoryAsync(int page, int pageSize)
        {

            var transactions = await _adPurchaseTransactionRepository.Get(
                includeProperties: "AdPurchaseItems.AdPackage,AdPurchaseItems.AdMedia,User"
            );

            if (transactions == null || !transactions.Any())
            {
                return (new List<AdPurchaseItemGetAllResponse>(), 0,0,0);
            }

            // Lấy tất cả AdPurchaseItems từ các transactions
            var allItems = transactions
                .Where(t => t.AdPurchaseItems != null)
                .SelectMany(t => t.AdPurchaseItems!)
                .OrderByDescending(i => i.CreatedDate)
                .ToList();



            var total = allItems.Count;
            var totalConsumed = allItems.Sum(i => i.ConsumedViewFee);
            var totalPurchased = allItems.Sum(i => i.Price);

            // Áp dụng paging
            var pagedItems = allItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedItems = _mapper.Map<List<AdPurchaseItemGetAllResponse>>(pagedItems);

            return (mappedItems, total, totalConsumed, totalPurchased);
        }

		public async Task<List<AdPurchaseItemGetAllResponse>> GetAllAdPurchaseItemById(string id)
		{
			var transaction = await _adPurchaseTransactionRepository.GetSingle(
				filter: t => t.AdPurchaseItems.Any(i => i.Id == id),

				includeProperties: "AdPurchaseItems.AdPackage,AdPurchaseItems.AdMedia,User"
			);


			if (transaction == null || transaction.AdPurchaseItems == null)
				return new List<AdPurchaseItemGetAllResponse>();

			var items = transaction.AdPurchaseItems
				.OrderByDescending(i => i.CreatedDate)
				.ToList();

			var mappedItems = _mapper.Map<List<AdPurchaseItemGetAllResponse>>(items);

			return mappedItems;
		}

	}

}
