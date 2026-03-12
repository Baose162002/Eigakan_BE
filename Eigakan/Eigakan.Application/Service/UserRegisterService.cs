using AutoMapper;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Enum;
using Eigakan.Domain.Models;
using Eigakan.Application.Interface;
using Eigakan.Domain.Request.UserRegisterRequest;
using Eigakan.Application.Helper.Logging;

namespace Eigakan.Application.Service
{
	public class UserRegisterService : IUserRegisterService
	{
		private readonly IUserRegisterRepository _userRegisterRepository;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;
		private readonly Logger _logger;

		public UserRegisterService(IUserRegisterRepository userRegisterRepository, IMapper mapper,
								   IUserRepository userRepository, Logger logger)
		{
			_userRegisterRepository = userRegisterRepository;
			_mapper = mapper;
			_userRepository = userRepository;
			_logger = logger;
		}

		public async Task<Result<UserRegister>> AcceptedUserRegister(AcceptedUserRegisterRequest acceptedUserRegisterRequest)
		{
			try
			{
				var existingUser = await _userRegisterRepository.GetUserRegisterById(acceptedUserRegisterRequest.Id);

				if (existingUser == null)
					return new Result<UserRegister> { Success = false, Message = "Id does not exist" };

				if(existingUser.Status == UserRegisterEnum.ACCEPTED.ToString() || existingUser.Status == UserRegisterEnum.REJECTED.ToString())
					return new Result<UserRegister> { Success = false, Message = "Can not update this register" };

				// Cập nhật trạng thái của user

				existingUser.Status = UserRegisterEnum.ACCEPTED.ToString();


				await _userRegisterRepository.Update(existingUser);

				return new Result<UserRegister>
				{
					Success = true,
					Message = "Update status successfull"
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(UserRegister));
				return new Result<UserRegister> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<UserRegister>> RejectedUserRegister(RejectedUserRegisterRequest rejectedUserRegisterRequest)
		{
			try
			{

				var existingUser = await _userRegisterRepository.GetUserRegisterById(rejectedUserRegisterRequest.Id);

				if (existingUser == null)
					return new Result<UserRegister> { Success = false, Message = "Id does not exist" };

				if (existingUser.Status == UserRegisterEnum.ACCEPTED.ToString() || existingUser.Status == UserRegisterEnum.REJECTED.ToString())
					return new Result<UserRegister> { Success = false, Message = "Can not update this register" };
				// Cập nhật trạng thái của user

				existingUser.Status = UserRegisterEnum.REJECTED.ToString();
				existingUser.ReasonForRejection = rejectedUserRegisterRequest.ReasonForRejection;

				await _userRegisterRepository.Update(existingUser);

				return new Result<UserRegister>
				{
					Success = true,
					Message = "Update status successfull"
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(UserRegister));
				return new Result<UserRegister> { Success = false, Message = ex.Message };
			}
		}

		public async Task<(List<UserRegister> Users, int Total)> GetAllUserRegisterAsync(int page, int pageSize, string? status, string? name)
		{
			// Lấy danh sách user với phân trang
			var listUser = await _userRegisterRepository.GetAllUserRegisterAsync(page, pageSize,status,name);

			// Đếm tổng số lượng user
			var total = await _userRegisterRepository.CountAllUserRegisterAsync();

			// Trả về dữ liệu và tổng số lượng
			return (_mapper.Map<List<UserRegister>>(listUser), total);
		}

		public async Task<List<UserRegister>> GetAllUserRegisterAsyncByEmail(string email)
		{
			// Lấy danh sách user với phân trang
			var listUser = await _userRegisterRepository.GetUserRegisterByEmail(email);

			// Trả về dữ liệu sau khi map
			return _mapper.Map<List<UserRegister>>(listUser);
		}

		public async Task<Result<UserRegister>> GetUserRegisterById(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
					return new Result<UserRegister> { Success = false, Message = "Id is not be null" };

				var userId = await _userRegisterRepository.GetUserRegisterById(id);

				if (userId == null)
					return new Result<UserRegister> { Success = false, Message = "Id does not exist" };

				return new Result<UserRegister>
				{
					Success = true,
					Data = _mapper.Map<UserRegister>(userId),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(UserRegister));
				return new Result<UserRegister> { Success = false, Message = ex.Message };
			}
		}
		
		public async Task<Result<UserRegister>> CreateUserRegister(UserRegisterCreateRequest userRegisterCreateRequest)
		{
			try
			{
				var newRegister = new UserRegister()
				{
					Id = Guid.NewGuid().ToString(),
					Email = userRegisterCreateRequest.Email,
					FullName = userRegisterCreateRequest.FullName,
					PhoneNumber = userRegisterCreateRequest.PhoneNumber,
					Reason = userRegisterCreateRequest.Reason,
					FileUrl = userRegisterCreateRequest.FileUrl,
					CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
					Status = UserRegisterEnum.REVIEWING.ToString()
				};

				await _userRegisterRepository.Insert(newRegister);
				return new Result<UserRegister> { Success = true, Message = "Create successfull", Data = newRegister };
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(UserRegister));
				return new Result<UserRegister> { Success = false, Message = ex.Message };
			}
		}
	}
}
