using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using Eigakan.Application.Helper;
using Microsoft.Extensions.Options;

namespace Eigakan.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UploadController : ControllerBase
	{
		private readonly IUploadService _uploadService;
		private readonly AwsS3Service _awsS3Service;
		private readonly BunnyStreamUploadService _bunnyStreamUploadService;
		private readonly HttpClient _httpClient;
		private readonly BunnyStreamSettings _settings;

		public UploadController(IUploadService uploadService, AwsS3Service awsS3Service, 
								BunnyStreamUploadService bunnyStreamUploadService, HttpClient httpClient, 
								IOptions<BunnyStreamSettings> settings)
		{
			_uploadService = uploadService;
			_awsS3Service = awsS3Service;
			_bunnyStreamUploadService = bunnyStreamUploadService;
			_settings = settings.Value;
		}

		[HttpGet("GetPreFileTemp")]
		[Authorize]
		public async Task<IActionResult> GetPreFileTemp(string Id, string fileName)
		{
			var results = await _awsS3Service.GetPreFileTemp(Id, fileName);

			if (!results.Success)
			{
				return BadRequest(new
				{
					results.Success,
					results.Message
				});
			}

			return Ok(new
			{
				results.Success,
				results.Message,
				results.Data
			});
		}

		[HttpGet("GetPreFileUrl")]
		[Authorize]
		public async Task<IActionResult> GetPreFileUrl(string userId, string fileName)
		{
			var results = await _awsS3Service.GetPreFileUrl(userId, fileName);

			if (!results.Success)
			{
				return BadRequest(new
				{
					results.Success,
					results.Message
				});
			}

			return Ok(new
			{
				results.Success,
				results.Message,
				results.Data
			});
		}

		[HttpGet("GetPreFileUrlMovie")]
		[Authorize]
		public async Task<IActionResult> GetPreFileUrlMovie(string movieId, string fileName)
		{
			var results = await _awsS3Service.GetPreFileUrlMovie(movieId, fileName);

			if (!results.Success)
			{
				return BadRequest(new
				{
					results.Success,
					results.Message
				});
			}

			return Ok(new
			{
				results.Success,
				results.Message,
				results.Data
			});
		}

		[HttpGet("GetPreFileContract")]
        [Authorize]
        public async Task<IActionResult> GetPreFileContract(string userId, string fileName)
        {
            var results = await _awsS3Service.GetPreFileContract(userId, fileName);

            if (!results.Success)
            {
                return BadRequest(new
                {
                    results.Success,
                    results.Message
                });
            }

            return Ok(new
            {
                results.Success,
                results.Message,
                results.Data
            });
        }

        [HttpPost("Upload_Pictures")]
		public async Task<IActionResult> CreatePictures([FromForm] IEnumerable<IFormFile> formFiles)
		{
			if (formFiles == null || !formFiles.Any())
			{
				return BadRequest(new
				{
					Status = false,
					Message = "No files provided"
				});
			}

			var resultList = new List<Result<UploadPictureResponse>>();

			foreach (var file in formFiles)
			{
				var results = await _uploadService.UploadPictureUserAsync(file);
				resultList.Add(results);
			}

			var successResults = resultList.Where(r => r.Success).Select(r => r.Data).ToList();
			var errorResults = resultList.Where(r => !r.Success).Select(r => r.Message).ToList();

			if (errorResults.Any())
			{
				return BadRequest(new
				{
					Status = false,
					Message = "Some files failed to upload",
					Errors = errorResults
				});
			}

			return Ok(new
			{
				Status = true,
				Message = "All files uploaded successfully",
				Data = successResults
			});
		}

		[HttpPost("UploadFileTemp")]
		public async Task<IActionResult> CreateFileUserRegister([FromForm] IEnumerable<IFormFile> formFiles)
		{
			if (formFiles == null || !formFiles.Any())
			{
				return BadRequest(new
				{
					Status = false,
					Message = "No files provided"
				});
			}

			var resultList = new List<Result<UploadPictureResponse>>();

			foreach (var file in formFiles)
			{
				var results = await _awsS3Service.UploadFileTempAsync(file);
				resultList.Add(results);
			}

			var successResults = resultList.Where(r => r.Success).Select(r => r.Data).ToList();
			var errorResults = resultList.Where(r => !r.Success).Select(r => r.Message).ToList();

			if (errorResults.Any())
			{
				return BadRequest(new
				{
					Status = false,
					Message = "Some files failed to upload",
					Errors = errorResults
				});
			}

			return Ok(new
			{
				Status = true,
				Message = "All files uploaded successfully",
				Data = successResults
			});
		}

		[HttpPost("UploadFileContractTemp")]
		public async Task<IActionResult> UploadFileContractTemp([FromForm] IEnumerable<IFormFile> formFiles)
		{
			if (formFiles == null || !formFiles.Any())
			{
				return BadRequest(new
				{
					Status = false,
					Message = "No files provided"
				});
			}

			var resultList = new List<Result<UploadPictureResponse>>();

			foreach (var file in formFiles)
			{
				var results = await _awsS3Service.UploadFileContractTempAsync(file);
				resultList.Add(results);
			}

			var successResults = resultList.Where(r => r.Success).Select(r => r.Data).ToList();
			var errorResults = resultList.Where(r => !r.Success).Select(r => r.Message).ToList();

			if (errorResults.Any())
			{
				return BadRequest(new
				{
					Status = false,
					Message = "Some files failed to upload",
					Errors = errorResults
				});
			}

			return Ok(new
			{
				Status = true,
				Message = "All files uploaded successfully",
				Data = successResults
			});
		}

		[HttpPost("upload_VideoBunny")]
		public async Task<IActionResult> UploadVideoBunny([FromBody] VideoCreateRequest request)
		{

			var videoUrl = await _bunnyStreamUploadService.UploadVideoAsync(request.Title);
			if (videoUrl != null)
			{
				return Ok(new { message = "Upload thành công!", videoUrl });
			}
			else
			{
				return StatusCode(500, "Có lỗi khi upload video.");
			}
		}

		public class VideoCreateRequest
		{
			public string Title { get; set; }
		}

	}
}