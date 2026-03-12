using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.ContractRequest;
using Eigakan.Domain.Response.ContractResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IContractService
    {
        Task<Result<string>> GenerateContractAsync(ContractGenerationRequest request);
        Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractAsync(int page, int pageSize, string? status, string? title);
		Task<Result<ContractGetAllResponse>> GetContractById(string id);
        Task<Result<string>> UpdateContractAsync(string contractId, ContractGenerationRequest request);
        Task<(List<ContractGetAllResponse> Contracts, int Total, int TotalSigned, decimal? TotalEarning)> GetAllContractByLogin(int page, int pageSize, string? status, string? title);
        Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractByUserId(string userId, int page, int pageSize, string? status, string? title);
		Task<Result<Contract>> DeniedContract(DeniedContractRequest deniedContractRequest);
        Task<Result<Contract>> AcceptedContract(AcceptContractRequest acceptContractRequest);
        Task<Result<string>> ExtendContractAsync(string originalContractId, ContractGenerationRequest request);
        Task<Result<ContractGetAllResponse>> RequestContractExtensionAsync(string contractId);
        Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractAsyncByMovie(string movieId, int page, int pageSize, string? status, string? title);
        Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractUserByMovie(string movieId, int page, int pageSize, string? status, string? title);
    }
}
