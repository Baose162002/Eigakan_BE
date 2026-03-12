using DocumentFormat.OpenXml.Spreadsheet;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.ContractRequest;
using Eigakan.Domain.Request.UserRegisterRequest;
using Eigakan.Domain.Response.ContractResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eigakan.API.Controllers
{
	[ApiController]
	[Route("api/contracts")]

	public class ContractController : ControllerBase
	{
		private readonly IContractService _contractService;

		private readonly ILogger<ContractController> _logger;

		public ContractController(IContractService contractService, ILogger<ContractController> logger)
		{
			_contractService = contractService;
			_logger = logger;
		}


		[HttpGet]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllContract(int page = 1, int pageSize = 10, string? status = null, string title = null)
		{
			var result = await _contractService.GetAllContractAsync(page, pageSize, status, title);

			return Ok(new
			{
				result.Total,
				result.Contracts
			});
		}

		[HttpGet("GetAllContractUserByLogin")]
		[Authorize]
		public async Task<IActionResult> GetAllContractUserByLogin(int page = 1, int pageSize = 10, string? status = null, string title = null)
		{

			var result = await _contractService.GetAllContractByLogin(page, pageSize, status, title);

			return Ok(new
			{
				result.Total,
				result.TotalSigned,
				result.TotalEarning,
				result.Contracts
			});
		}

		[HttpGet("GetAllContractByUserId")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllContractByUserId(string userId, int page = 1, int pageSize = 10, string? status = null, string title = null)
		{

			var result = await _contractService.GetAllContractByUserId(userId, page, pageSize, status, title);

			return Ok(new
			{
				result.Total,
				result.Contracts
			});
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> GetContractById(string id)
		{
			var contracts = await _contractService.GetContractById(id);
			return Ok(contracts);
		}

		[HttpPut("{contractId}")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> UpdateContractAsync(string contractId, [FromBody] ContractGenerationRequest request)
		{
			if (request == null)
			{
				return BadRequest(new { Success = false, Message = "Request body cannot be null" });
			}

			// Call the service to generate the contract
			var result = await _contractService.UpdateContractAsync(contractId, request);

			if (result.Success)
			{
				// If the contract was successfully generated and uploaded
				return Ok(new { Succees = result.Success, Message = result.Message, Data = result.Data });
			}
			else
			{
				return BadRequest(new { Succees = result.Success, Message = result.Message });
			}
		}

		[HttpPatch("Accepted_Contract")]
		[Authorize]
		public async Task<IActionResult> AcceptedUserRegister(AcceptContractRequest acceptContractRequest)
		{
			var results = await _contractService.AcceptedContract(acceptContractRequest);
			if (results.Success != false)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
				});
			}
			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

		[HttpPatch("Denied_Contract")]
		[Authorize]
		public async Task<IActionResult> RejectedUserRegister(DeniedContractRequest deniedContractRequest)
		{
			var results = await _contractService.DeniedContract(deniedContractRequest);
			if (results.Success != false)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
				});
			}
			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

		[HttpPost("Generate_Contract")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GenerateContractAsync([FromBody] ContractGenerationRequest request)
		{
			if (request == null)
			{
				return BadRequest(new { Success = false, Message = "Request body cannot be null" });
			}

			// Call the service to generate the contract
			var result = await _contractService.GenerateContractAsync(request);

			if (result.Success)
			{
				// If the contract was successfully generated and uploaded
				return Ok(new { Succees = result.Success, Message = result.Message, Data = result.Data });
			}
			else
			{
				return BadRequest(new { Succees = result.Success, Message = result.Message });
			}
		}
		
		[HttpPost("extend/{originalContractId}")]
		public async Task<IActionResult> ExtendContract(string originalContractId, [FromBody] ContractGenerationRequest request)
		{
			var result = await _contractService.ExtendContractAsync(originalContractId, request);
			{
				if (!result.Success)
					_logger.LogWarning("Failed to extend contract {ContractId}: {Message}", originalContractId, result.Message);
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPatch("{contractId}/request-extension")]
		public async Task<IActionResult> RequestExtension(string contractId)
		{
			var result = await _contractService.RequestContractExtensionAsync(contractId);
			{
				if (!result.Success)
					return BadRequest(result);
			}
			return Ok(result);
		}


		[HttpGet("GetAllContractByMovie/{movieId}")]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> GetAllContractByMovie(string movieId, int page = 1, int pageSize = 10, string? status = null, string title = null)
		{
			var result = await _contractService.GetAllContractAsyncByMovie(movieId, page, pageSize, status, title);

			return Ok(new
			{
				result.Total,
				result.Contracts
			});
		}

		[HttpGet("GetAllContractUserByMovie/{movieId}")]
		[Authorize]
		public async Task<IActionResult> GetAllContractUserByMovie(string movieId, int page = 1, int pageSize = 10, string? status = null, string title = null)
		{

			var result = await _contractService.GetAllContractUserByMovie(movieId, page, pageSize, status, title);
			return Ok(new
			{
				result.Total,
				result.Contracts
			});

		}
	}
}
