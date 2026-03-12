using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Discord;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRequest;
using Eigakan.Domain.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly PasswordSettings _passwordSettings;
		private readonly IConfiguration _configuration;
		private readonly IEmailService _emailService;
		private readonly Webhook _webhook;
		private readonly Logger _logger;
		private readonly IUserRegisterRepository _userRegisterRepository;
		private readonly AmazonS3Client _s3Client;

		public UserService(IUserRepository userRepository, IMapper mapper,
						   IHttpContextAccessor httpContextAccessor,
						   PasswordSettings passwordSettings, IConfiguration configuration,
						   IEmailService emailService, Webhook webhook,
						   Logger logger, IUserRegisterRepository userRegisterRepository)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
			_passwordSettings = passwordSettings;
			_configuration = configuration;
			_emailService = emailService;
			_webhook = webhook;
			_logger = logger;
			_userRegisterRepository = userRegisterRepository;
			var accessKey = configuration["AWS:AccessKey"];
			var secretKey = configuration["AWS:SecretKey"];
			var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);

			_s3Client = new AmazonS3Client(accessKey, secretKey, region);
		}

		public async Task<(List<UserGetAllResponse> Users, int Total)> GetAllUserAsync(int page, int pageSize, string? status, string? name,string? roleName)
		{
			
			var listUser = await _userRepository.GetAllUserAsync(page, pageSize,status,name,roleName);			
			var total = await _userRepository.CountAllUsersAsync();

			return (_mapper.Map<List<UserGetAllResponse>>(listUser), total);
		}

		public async Task<Result<UserGetAllResponse>> GetUserByEmail(string email)
		{
			try
			{
				if (string.IsNullOrEmpty(email))
					return new Result<UserGetAllResponse> { Success = false, Message = "Email is not be null" };

				var userId = await _userRepository.GetUserByEmail(email);

				if (userId == null)
					return new Result<UserGetAllResponse> { Success = false, Message = "Email does not exist" };

				return new Result<UserGetAllResponse>
				{
					Success = true,
					Data = _mapper.Map<UserGetAllResponse>(userId),
				};
			}
			catch (Exception ex)
			{
				return new Result<UserGetAllResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<UserGetAllResponse>> GetUserById(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
					return new Result<UserGetAllResponse> { Success = false, Message = "Id is not be null" };

				var userId = await _userRepository.GetUserById(id);

				if (userId == null)
					return new Result<UserGetAllResponse> { Success = false, Message = "Id does not exist" };

				return new Result<UserGetAllResponse>
				{
					Success = true,
					Data = _mapper.Map<UserGetAllResponse>(userId),
				};
			}
			catch (Exception ex)
			{
				return new Result<UserGetAllResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<UserGetAllResponse>> ChangeStatusUser(UserStatusRequest userStatusRequest)
		{
			try
			{

				var existingUser = await _userRepository.GetUserById(userStatusRequest.Id);

				if (existingUser == null)
					return new Result<UserGetAllResponse> { Success = false, Message = "Id does not exist" };

				// Cập nhật trạng thái của user

				existingUser.Status = userStatusRequest.Status == 0
				   ? UserStatusEnum.NORMAL.ToString()
				   : UserStatusEnum.INACTIVE.ToString();


				await _userRepository.Update(existingUser);

				return new Result<UserGetAllResponse>
				{
					Success = true,
					Message = "Update status successfull"
				};
			}
			catch (Exception ex)
			{
				return new Result<UserGetAllResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<UserGetAllResponse>> UpdateUser(string id, UserUpdateRequest userUpdateRequest)
		{
			try
			{
				var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
				var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

				var existingUser = await _userRepository.GetUserById(id);
				if (existingUser == null)
					return new Result<UserGetAllResponse> { Success = false, Message = "Id does not exist" };

				if (existingUser.Id == userId?.Value || roleClaim.Value == "ADMIN")
				{				
					existingUser.FullName = userUpdateRequest.FullName;
					existingUser.Birthday = userUpdateRequest.Birthday;
					existingUser.Gender = userUpdateRequest?.Gender;
					existingUser.Picture = userUpdateRequest?.Picture;

					await _userRepository.Update(existingUser);
					//await _logger.LogAnnoucement(existingUser, nameof(UserService));
					return new Result<UserGetAllResponse> { Data = _mapper.Map<UserGetAllResponse>(existingUser), Success = true, Message = "Update user successfull" };
					
				}
				return new Result<UserGetAllResponse> { Success = false, Message = "You do not have permission to update this user" };

			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(UserService));
				return new Result<UserGetAllResponse> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<UserGetAllResponse>> GetUserByLogin()
		{
			var userId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
			var existingUser = await _userRepository.GetUserById(userId?.Value);
			if (existingUser == null)
				return new Result<UserGetAllResponse> { Success = false, Message = "Id does not exist" };
			return new Result<UserGetAllResponse> { Data = _mapper.Map<UserGetAllResponse>(existingUser), Success = true, Message = "Get user successfull" };
		}

		//tạo user từ userregister
		public async Task<Result<UserGetAllResponse>> CreateUserByRegister(UserCreateRequest userCreateRequest)
		{
			await using var transaction = await _userRepository.BeginTransactionAsync();
			try
			{
				var existingUser = await _userRepository.GetUserByEmail(userCreateRequest.Email);
				if (existingUser != null)
				{
					return new Result<UserGetAllResponse>
					{
						Success = false,
						Message = "Email has been registered and cannot join this program"
					};
				}

				_passwordSettings.CreatePasswordHash("11111111",
					out byte[] passwordHash,
					out byte[] passwordSalt);

				var newUsers = new User
				{
					Id = Guid.NewGuid().ToString(),
					Email = userCreateRequest.Email,
					FullName = userCreateRequest.FullName,
					Birthday = "2025-01-1",
					Gender = true,
					Picture = "https://res.cloudinary.com/dn8bn2sty/image/upload/v1739785049/xrvkj1mpmtcngzamplgc.png",
					CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
					Status = UserStatusEnum.NORMAL.ToString(),
					PasswordHash = passwordHash,
					PasswordSalt = passwordSalt,
					RoleId = userCreateRequest.RoleId,
					UserRegisterId = userCreateRequest.UserRegisterId
				};

				await _userRepository.InsertTransaction(newUsers);


				var userRegister = await _userRegisterRepository.GetUserRegisterById(userCreateRequest.UserRegisterId);
				if (userRegister == null || userRegister.Status == UserRegisterEnum.REVIEWING.ToString() ||  userRegister.Status == UserRegisterEnum.REJECTED.ToString() )
				{
					return new Result<UserGetAllResponse>
					{
						Success = false,
						Message = "UserRegister does not exist or status is not reviewing"
					};
				}
				
				var updateUrlUser = await MoveFileToUserFolderAsync(userRegister.FileUrl, newUsers.Id);
				userRegister.FileUrl = updateUrlUser;

				await _userRegisterRepository.UpdateTransaction(userRegister);

				//thua ko biết savechange 1 lần duy nhất 
				await _userRegisterRepository.SaveChangeTransaction(); 
				await _userRepository.SaveChangeTransaction(); 
				await transaction.CommitAsync();

				string frontendreseturl = _configuration["FrontendSettings:VerifyAccountUrl"];

				var mailrequest = new EmailSetting.MailResponse
				{
					ToEmail = newUsers.Email,
					Subject = "Welcome to Eigakan",
					Body = $@"
						<div style='font-family:Arial, sans-serif; display:flex; justify-content:center;'>
						  <div style='max-width: 600px; border: 1px solid #e0e0e0; border-radius: 10px; padding: 20px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
							  <div style='text-align:center; padding:20px;'>
								  <img src='https://res.cloudinary.com/dn8bn2sty/image/upload/v1739771796/image_vxdaik.png' alt='Eigakan logo' style='width: 250px; margin-bottom:10px;'/>
							  </div>
							  <h2 style='text-align:center;'>Welcome to Eigakan</h2>
							  <p style='text-align:center;'> This is you account information - please change password at your first time login </p>
							  <p style='text-align:center;'> Email: {newUsers.Email} </p>
							  <p style='text-align:center;'> Password: 11111111 </p>
							  <p style='text-align:center;'>Eigakan</p>
						  </div>
						</div>",

				};
				var emailTask = Task.Run(() => _emailService.SendEmailAsync(mailrequest));
				// Đợi các tác vụ không đồng bộ kết thúc
				await emailTask;

				var userResponse = await _userRepository.GetUserById(newUsers.Id);

				return new Result<UserGetAllResponse>
				{
					Success = true,
					Data = _mapper.Map<UserGetAllResponse>(userResponse),
					Message = "Create successful"
				};
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				_logger.LogError(ex, nameof(UserService));
				return new Result<UserGetAllResponse>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		//di chuyển folder tạm sang chính với uid user
		private async Task<string> MoveFileToUserFolderAsync(string tempFileUrl, string userId)
		{
			var tempFileMatch = Regex.Match(tempFileUrl, @".*/temp-uploads/(?<id>[a-f0-9-]+)/(?<filename>.+)");
			if (tempFileMatch.Success)
			{
				var fileId = tempFileMatch.Groups["id"].Value;
				var fileName = tempFileMatch.Groups["filename"].Value;

				var sourceKey = $"temp-uploads/{fileId}/{fileName}";
				var destinationKey = $"user-uploads/{userId}/{fileName}"; 

				// Copy file từ temp-uploads vào user-uploads/{userId}/
				var copyRequest = new CopyObjectRequest
				{
					SourceBucket = "file-eigakan",
					DestinationBucket = "file-eigakan",
					SourceKey = sourceKey,
					DestinationKey = destinationKey
				};

				await _s3Client.CopyObjectAsync(copyRequest);

				// Xóa file trong temp-uploads
				await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
				{
					BucketName = "file-eigakan",
					Key = sourceKey
				});


				return $"https://file-eigakan.s3.ap-southeast-2.amazonaws.com/{destinationKey}";
			}
		
			throw new ArgumentException("Invalid temp file URL", nameof(tempFileUrl));
		}

		public async Task<Result<UserGetAllResponse>> CreateUser(UserCreateRequest userCreateRequest)
		{			
			try
			{
				var existingUser = await _userRepository.GetUserByEmail(userCreateRequest.Email);
				if (existingUser != null)
				{
					return new Result<UserGetAllResponse>
					{
						Success = false,
						Message = "Email has been registered and cannot join this program"
					};
				}

				_passwordSettings.CreatePasswordHash("11111111",
					out byte[] passwordHash,
					out byte[] passwordSalt);

				var newUsers = new User
				{
					Id = Guid.NewGuid().ToString(),
					Email = userCreateRequest.Email,
					FullName = userCreateRequest.FullName,
					Birthday = "2025-01-1",
					Gender = true,
					Picture = "https://res.cloudinary.com/dn8bn2sty/image/upload/v1739785049/xrvkj1mpmtcngzamplgc.png",
					CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
					Status = UserStatusEnum.NORMAL.ToString(),
					PasswordHash = passwordHash,
					PasswordSalt = passwordSalt,
					RoleId = userCreateRequest.RoleId,
					UserRegisterId = null,
				};
				await _userRepository.Insert(newUsers);
				
				var userResponse = await _userRepository.GetUserById(newUsers.Id);

				return new Result<UserGetAllResponse>
				{
					Success = true,
					Data = _mapper.Map<UserGetAllResponse>(userResponse),
					Message = "Create successful"
				};
			}
			catch (Exception ex)
			{			
				await _logger.LogError(ex, nameof(UserService));
				return new Result<UserGetAllResponse>
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

	}
}
