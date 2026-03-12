using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Response.AdminDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class AdminDashboardService : IAdminDashboardService
	{
		private readonly IUserRepository _userRepository;
		private readonly IMoviesRepository _moviesRepository;
		private readonly IUserRegisterRepository _userRegisterRepository;
		private readonly IContractRepository _contractRepository;

		public AdminDashboardService(IUserRepository userRepository, IMoviesRepository moviesRepository,
									 IUserRegisterRepository userRegisterRepository, IContractRepository contractRepository)
		{
			_userRepository = userRepository;
			_moviesRepository = moviesRepository;
			_userRegisterRepository = userRegisterRepository;
			_contractRepository = contractRepository;
		}

		public async Task<Result<AdminDasboardOverallResponse>> DashboardAdminOverall()
		{
			var userCount = await _userRepository.CountAllUsersAsync();
			var userActiveCount = await _userRepository.CountAllUsersActiveAsync();

			var moviesCount = await _moviesRepository.CountAllMovieAsync();
			var moviesActiveCount = await _moviesRepository.CountAllMovieActiveAsync();

			var userRegisterCount = await _userRegisterRepository.CountAllUserRegisterAsync();
			var userRegisterAcceptedCount = await _userRegisterRepository.CountAllUserRegisterAcceptedAsync();

			var contractCount = await _contractRepository.CountAllContractAsync();
			var contractSignedCount = await _contractRepository.CountAllContractSignedAsync();

			var result = new AdminDasboardOverallResponse
			{
				TotalUsers = userCount,
				ActiveUsers = userActiveCount,
				TotalMovies = moviesCount,
				ActiveMovies = moviesActiveCount,
				TotalUserRegisters = userRegisterCount,
				AcceptedUserRegisters = userRegisterAcceptedCount,
				TotalContracts = contractCount,
				SignedContracts = contractSignedCount
			};

			return new Result<AdminDasboardOverallResponse>
			{
				Data = result,
				Success = true,
				Message = "Success"
			};
		}


	}

}
