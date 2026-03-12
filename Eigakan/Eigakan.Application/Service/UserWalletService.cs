using AutoMapper;
using Eigakan.Application.Helper;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response.Media;
using Eigakan.Domain.Response.UserWallet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
    public class UserWalletService : IUserWalletService
    {
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public UserWalletService(IUserWalletRepository userWalletRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _userWalletRepository = userWalletRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        
        public async Task<Result<UserWalletGetAllResponse>> GetUserWalletById()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new Result<UserWalletGetAllResponse>
                    {
                        Success = false,
                        Message = "User not authenticated.",
                    };
                }
                var userWallet = await _userWalletRepository.GetUserWalletById(userId);

                if (userWallet == null)
                {
                    return new Result<UserWalletGetAllResponse>
                    {
                        Success = false,
                        Message = "User Wallet not found",

                    };
                }

                return new Result<UserWalletGetAllResponse>
                {
                    Success = true,
                    Message = "Success",
                    Data = _mapper.Map<UserWalletGetAllResponse>(userWallet),
                };
            }
            catch (Exception ex)
            {
                return new Result<UserWalletGetAllResponse> { Success = false, Message = ex.Message };
            }
        }
    }
}
