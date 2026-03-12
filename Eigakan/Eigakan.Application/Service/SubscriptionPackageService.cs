using AutoMapper;
using CloudinaryDotNet;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.ContractRequest;
using Eigakan.Domain.Request.SubscriptionPackageRequest;
using Eigakan.Domain.Response;
using Eigakan.Domain.Response.ContractResponse;
using Eigakan.Domain.Response.SubscriptionPackageResponse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class SubscriptionPackageService : ISubscriptionPackageService
    {
        private readonly ILogger<SubscriptionPackageService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ISubscriptionPackageRepository _subscriptionPackageRepositories;
        private readonly string _templateFilePath;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SubscriptionPackageService( ISubscriptionPackageRepository subscriptionPackageRepositories,IMapper mapper,
                                           ILogger<SubscriptionPackageService> logger)
        {

            _mapper = mapper;
            _subscriptionPackageRepositories = subscriptionPackageRepositories;
            _logger = logger;
        }
        public async Task<Result<(List<SubscriptionPackageGetAllResponse> SubscriptionPackages, int Total)>> GetAllSubscriptionPackageAsync(int page, int pageSize)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return new Result<(List<SubscriptionPackageGetAllResponse> SubscriptionPackage, int Total)>
                    {
                        Success = false,
                        Message = "Invalid page or pageSize values.",
                        Data = (null, 0)
                    };
                }

                var listSubscriptionPackage = await _subscriptionPackageRepositories.GetAllSubscriptionPackage(page, pageSize);

                var total = await _subscriptionPackageRepositories.CountAllSubscriptionPackageAsync();

                var response = _mapper.Map<List<SubscriptionPackageGetAllResponse>>(listSubscriptionPackage);

                return new Result<(List<SubscriptionPackageGetAllResponse> SubscriptionPackage, int Total)>
                {
                    Success = true,
                    Message = "SubscriptionPackages retrieved successfully.",
                    Data = (response, total)
                };
            }
            catch (Exception ex)
            {
                
                return new Result<(List<SubscriptionPackageGetAllResponse> SubscriptionPackage, int Total)>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving SubscriptionPackage: {ex.Message}",
                    Data = (null, 0)
                };
            }
        }
       
        public async Task<Result<SubscriptionPackageGetAllResponse>> GetSubscriptionPackageById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = false,
                    Message = "Id must not be null or empty."
                };
            }

            try
            {
                var contract = await _subscriptionPackageRepositories.GetSubscriptionPackageById(id);

                if (contract == null)
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "SubscriptionPackage with the specified Id does not exist."
                    };
                }

                var response = _mapper.Map<SubscriptionPackageGetAllResponse>(contract);

                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = true,
                    Message = "SubscriptionPackage retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the SubscriptionPackage: {ex.Message}"
                };
            }
        }

        public async Task<Result<SubscriptionPackageGetAllResponse>> CreateSubscriptionPackageAsync(SubscriptionPackageCreateRequest request)
        {
            try
            {            
                if (string.IsNullOrWhiteSpace(request.PackageName))
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "PackageName cannot be empty."
                    };
                }
                
                if (request.Price == null || request.Price.Value <= 0)
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "Price must be greater than 0."
                    };
                }
                if (request.Duration == null || request.Duration.Value <= 0)
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "Duration must be greater than 0."
                    };
                }

                var shortGuid = Guid.NewGuid().ToString(); // GUID gốc: 36 ký tự
                shortGuid = string.Join("-", shortGuid.Split('-').Select(part => part.Substring(0, 4))); 

                // 4. Lưu thông tin vào database
                var subscriptionPackage = new SubscriptionPackage
                {
                    Id = shortGuid,
                    PackageName = request.PackageName,
                    Price = request.Price.Value,
                    Duration = request.Duration,
                    UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    Status = "Active",
                    
                };

                await _subscriptionPackageRepositories.Insert(subscriptionPackage);

                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = true,
                    Message = "SubscriptionPackage generated and uploaded successfully",   
                    Data = _mapper.Map<SubscriptionPackageGetAllResponse>(subscriptionPackage)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating contract for request: {@Request}", request);
                return new Result<SubscriptionPackageGetAllResponse> { Success = false, Message = $"Failed to generate contract: {ex.Message}" };
            }
        }

        public async Task<Result<SubscriptionPackageGetAllResponse>> UpdateSubscriptionPackageAsync(string subscriptionpackageId, SubscriptionPackageUpdateRequest request)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(request.PackageName))
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "PackageName cannot be empty."
                    };
                }

                if (request.Price == null || request.Price.Value <= 0)
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "Price must be greater than 0."
                    };
                }
                if (request.Duration == null || request.Duration.Value <= 0)
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "Duration must be greater than 0."
                    };
                }

                var existingSubscriptionPackage = await _subscriptionPackageRepositories.GetSubscriptionPackageById(subscriptionpackageId);
                if (existingSubscriptionPackage == null)
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "Invalid subscriptionpackageId: SubscriptionPackage does not exist."
                    };
                }

                existingSubscriptionPackage.PackageName = request.PackageName;           
                existingSubscriptionPackage.Price = request.Price.Value;
                existingSubscriptionPackage.Duration = request.Duration;
                existingSubscriptionPackage.UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));



                await _subscriptionPackageRepositories.Update(existingSubscriptionPackage);

                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = true,
                    Message = "SubscriptionPackage updated and uploaded successfully.",
                    Data = _mapper.Map<SubscriptionPackageGetAllResponse>(existingSubscriptionPackage)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SubscriptionPackage with Id: {subscriptionpackageId}, Request: {@Request}", subscriptionpackageId, request);
                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = false,
                    Message = $"Failed to update contract: {ex.Message}"
                };
            }



        }

        public async Task<Result<SubscriptionPackageGetAllResponse>> UpdateSubscriptionPackageStatusAsync(string subscriptionpackageId)
        {
            try
            {
                var existingSubscriptionPackage = await _subscriptionPackageRepositories.GetSubscriptionPackageById(subscriptionpackageId);
                if (existingSubscriptionPackage == null)
                {
                    return new Result<SubscriptionPackageGetAllResponse>
                    {
                        Success = false,
                        Message = "Invalid subscriptionpackageId: SubscriptionPackage does not exist."
                    };
                }

                existingSubscriptionPackage.Status = existingSubscriptionPackage.Status == "Archived" ? "Active" : "Archived";




                await _subscriptionPackageRepositories.Update(existingSubscriptionPackage);

                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = true,
                    Message = "SubscriptionPackage updated and uploaded successfully.",
                    Data = _mapper.Map<SubscriptionPackageGetAllResponse>(existingSubscriptionPackage)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SubscriptionPackage with Id: {subscriptionpackageId}, Request: {@Request}", subscriptionpackageId);
                return new Result<SubscriptionPackageGetAllResponse>
                {
                    Success = false,
                    Message = $"Failed to update contract: {ex.Message}"
                };
            }



        }
    }
}
