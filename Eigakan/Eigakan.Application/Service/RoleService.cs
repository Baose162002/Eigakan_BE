using AutoMapper;
using CloudinaryDotNet.Actions;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class RoleService : IRoleService
	{
		private readonly IRoleRepository _roleRepository;
		private readonly IMapper _mapper;

		public RoleService(IRoleRepository roleRepository, IMapper mapper) 
		{
			_roleRepository = roleRepository;
			_mapper = mapper;
		}
		public async Task<(List<Domain.Models.Role> Roles, int Total)> GetAllRoleAsync(int page, int pageSize)
		{
			// Lấy danh sách user với phân trang
			var listRole = await _roleRepository.GetAllRoleAsync(page, pageSize);

			// Đếm tổng số lượng user
			var total = await _roleRepository.CountAllRolesAsync();

			// Trả về dữ liệu và tổng số lượng
			return (_mapper.Map<List<Domain.Models.Role>>(listRole), total);
		}
	}
}
