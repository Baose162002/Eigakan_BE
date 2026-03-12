using Eigakan.Domain.Models;


namespace Eigakan.Application.Interface.IRepository
{
    public interface IContractRepository : IGenericRepository<Contract>
    {
        Task<List<Domain.Models.Contract>> GetAllContract(int page, int pageSize, string? status, string? title);
        Task<Domain.Models.Contract> GetContractById(string id);
        Task<int> CountAllContractAsync();
		Task<int> CountAllContractSignedAsync();

		Task<int> CountAllContractByUserIdAsync(string userId);
        Task<List<Domain.Models.Contract>> GetAllContractUserById(int page, int pageSize, string id, string? status, string? title);
		Task<List<Domain.Models.Contract>> GetAllContractNoPagingByUserId(string userId);


        Task<List<Domain.Models.Contract>> GetAllContractsForCheckExpirationAsync();
        Task UpdateContractStatusAsync(string contractId, string newStatus);
        Task<List<Domain.Models.Contract>> GetExpiredContractsAsync();
        Task<List<Domain.Models.Contract>> GetAllContractByMovie(string movieId, int page, int pageSize, string? status, string? title);
        Task<List<Domain.Models.Contract>> GetAllContractUserByMovieId(int page, int pageSize, string id, string movieId, string? status, string? title);
        Task<int> CountAllContractByUserIdAndMovieIdAsync(string userId, string movieId);
        Task<int> CountAllContractByMovieIdAsync(string movieId);
    }
}
