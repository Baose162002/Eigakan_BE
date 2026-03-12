using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.ContractRepositories
{
    public class ContractRepository : GenericBase<Domain.Models.Contract>, IContractRepository
    {
        public async Task<List<Domain.Models.Contract>> GetAllContract(int page, int pageSize, string? status,string? title)
        {
            var contracts = await Get(
                orderBy: q => q.OrderByDescending(u => u.CreateDate),
                includeProperties: "User,Movie,Movie.Media",
                 filter: q => (string.IsNullOrEmpty(status) || q.Status == status) &&
                     (string.IsNullOrEmpty(title) || q.Movie.Title.Contains(title) || q.Movie.OriginName.Contains(title)),
                pageIndex: page,
				pageSize: pageSize);
            return contracts.OrderByDescending(u => u.CreateDate).ToList();
        }

        public async Task<int> CountAllContractAsync()
        {
            return await CountAsync();
        }

		public async Task<int> CountAllContractSignedAsync()
		{
			return await CountAsync(p => p.Status == "SIGNED");
		}

		public async Task<int> CountAllContractByUserIdAsync(string userId)
		{
			return await CountAsync(c => c.UserId == userId);
		}

		public async Task<Domain.Models.Contract> GetContractById(string id)
        {
            return await GetSingle(filter: c => c.Id == id, includeProperties: "Movie,User,Movie.Media");
        }

        public async Task<List<Domain.Models.Contract>> GetAllContractUserById(int page, int pageSize, string id, string? status, string? title)
        {
            var contracts = await Get(
				orderBy: q => q.OrderByDescending(u => u.CreateDate),
				filter: c => c.UserId == id && (string.IsNullOrEmpty(status) || c.Status == status) &&
                     (string.IsNullOrEmpty(title) || c.Movie.Title.Contains(title) || c.Movie.OriginName.Contains(title)), 
                includeProperties: "Movie,User,Movie.Media",
				pageIndex: page,
				pageSize: pageSize);
            return contracts.ToList();
        }

		public async Task<List<Domain.Models.Contract>> GetAllContractNoPagingByUserId(string userId)
		{
			var contracts = await Get(
				filter: c => c.UserId == userId,
				includeProperties: "Movie,User,Movie.Media");
			return contracts.ToList();
		}

        public async Task<List<Domain.Models.Contract>> GetAllContractsForCheckExpirationAsync()
        {

            var contracts = await Get(includeProperties: "User,Movie,Movie.Media");
            return contracts.ToList();
        }

        public async Task UpdateContractStatusAsync(string contractId, string newStatus)
        {
            var contract = await GetContractById(contractId);
            if (contract != null)
            {
                contract.Status = newStatus;
                await Update(contract);
            }
        }
        public async Task<List<Domain.Models.Contract>> GetExpiredContractsAsync()
        {
            var now = DateTime.UtcNow.Date;

            var contracts = await Get(
                filter: c => c.EndDate.HasValue &&
                             c.EndDate.Value.Date <= now &&
                             c.Status != "Inactive",
                includeProperties: "Movie"
            );

            return contracts.ToList();
        }

        public async Task<int> CountAllContractByMovieIdAsync(string movieId)
        {
            return await CountAsync(c => c.MovieId == movieId);
        }
        public async Task<int> CountAllContractByUserIdAndMovieIdAsync(string userId, string movieId)
        {
            return await CountAsync(c =>
                c.UserId == userId &&
                c.MovieId == movieId
            );
        }

        public async Task<List<Domain.Models.Contract>> GetAllContractByMovie(string movieId, int page, int pageSize, string? status, string? title)
        {
            var contracts = await Get(
                includeProperties: "User,Movie,Movie.Media",
                 filter: q => q.MovieId == movieId && (string.IsNullOrEmpty(status) || q.Status == status) &&
                     (string.IsNullOrEmpty(title) || q.Movie.Title.Contains(title) || q.Movie.OriginName.Contains(title)),
                pageIndex: page,
                pageSize: pageSize);
            return contracts.OrderByDescending(u => u.CreateDate).ToList();
        }
        public async Task<List<Domain.Models.Contract>> GetAllContractUserByMovieId(int page, int pageSize, string id, string movieId, string? status, string? title)
        {
            var contracts = await Get(
                filter: c => c.UserId == id && c.MovieId == movieId && (string.IsNullOrEmpty(status) || c.Status == status) &&
                     (string.IsNullOrEmpty(title) || c.Movie.Title.Contains(title) || c.Movie.OriginName.Contains(title)),
                includeProperties: "Movie,User,Movie.Media",
                pageIndex: page,
                pageSize: pageSize);

            return contracts.OrderByDescending(u => u.CreateDate).ToList();
        }
    }

}
