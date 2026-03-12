using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Eigakan.Application.Helper;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Response;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using System.Security.Claims;
using Eigakan.Application.Helper.Logging;

namespace Eigakan.Application.Service
{
	public class AwsS3Service 
	{
		private readonly AmazonS3Client _s3Client;
		private readonly string _bucketName;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AwsS3Service(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			var accessKey = configuration["AWS:AccessKey"];
			var secretKey = configuration["AWS:SecretKey"];
			var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);

			_s3Client = new AmazonS3Client(accessKey, secretKey, region);
			_bucketName = configuration["AWS:BucketName"];
			_httpContextAccessor = httpContextAccessor;
		}

		//lưu tạm thời trong aws 
		public async Task<Result<UploadPictureResponse>> UploadFileTempAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new Exception("File không hợp lệ");

			string key = $"temp-uploads/{Guid.NewGuid().ToString()}/{file.FileName}";

			using var stream = file.OpenReadStream();
			var request = new PutObjectRequest
			{
				BucketName = _bucketName,
				Key = key,
				InputStream = stream,
				ContentType = file.ContentType
			};

			await _s3Client.PutObjectAsync(request);
			return new Result<UploadPictureResponse>
			{
				Success = true,
				Data = new UploadPictureResponse
				{
					Url = $"https://{_bucketName}.s3.amazonaws.com/{key}"
				},
				Message = "Upload Successful!"
			};
		}

		public async Task<Result<UploadPictureResponse>> UploadFileContractTempAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new Exception("File không hợp lệ");

			string key = $"contract-template/{file.FileName}";

			using var stream = file.OpenReadStream();
			var request = new PutObjectRequest
			{
				BucketName = _bucketName,
				Key = key,
				InputStream = stream,
				ContentType = file.ContentType
			};

			await _s3Client.PutObjectAsync(request);
			return new Result<UploadPictureResponse>
			{
				Success = true,
				Data = new UploadPictureResponse
				{
					Url = $"https://{_bucketName}.s3.amazonaws.com/{key}"
				},
				Message = "Upload Successful!"
			};
		}

		public async Task<Result<UploadPictureResponse>> GetPreFileTemp(string Id, string fileName)
		{
			try
			{

				string bucketName = "file-eigakan";
				string key = $"temp-uploads/{Id}/{fileName}";

				var request = new GetPreSignedUrlRequest
				{
					BucketName = bucketName,
					Key = key,
					Expires = DateTime.UtcNow.AddMinutes(30)
				};

				string presignedUrl = _s3Client.GetPreSignedURL(request);

				return new Result<UploadPictureResponse>
				{
					Success = true,
					Data = new UploadPictureResponse
					{
						Url = presignedUrl
					},
					Message = "Get PreSignedUrl Successful!"
				};

			}
			catch (Exception ex)
			{
				//await _logger.LogError(ex, nameof(AwsS3Service));
				return new Result<UploadPictureResponse>
				{
					Success = false,
					Message = ex.Message
				};
			}


		}

		public async Task<Result<UploadPictureResponse>> GetPreFileUrl(string userId, string fileName)
		{
			try
			{
				var userIdclaim = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
				var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

				if (userId != userIdclaim.Value && roleClaim.Value != "ADMIN")
				{
					return new Result<UploadPictureResponse>
					{
						Success = false,
						Message = "You have not permission here"
					};
				}

				string bucketName = "file-eigakan";
				string key = $"user-uploads/{userId}/{fileName}";

				var request = new GetPreSignedUrlRequest
				{
					BucketName = bucketName,
					Key = key,
					Expires = DateTime.UtcNow.AddMinutes(30) 
				};

				string presignedUrl = _s3Client.GetPreSignedURL(request);
				
				return new Result<UploadPictureResponse>
				{
					Success = true,
					Data = new UploadPictureResponse
					{
						Url = presignedUrl
					},
					Message = "Get PreSignedUrl Successful!"
				};

			}
			catch(Exception ex)
			{
				//await _logger.LogError(ex, nameof(AwsS3Service));
				return new Result<UploadPictureResponse>
				{					
					Success = false,
					Message = ex.Message
				};
			}


		}

		public async Task<Result<UploadPictureResponse>> GetPreFileUrlMovie(string movieId, string fileName)
		{
			try
			{

				string bucketName = "file-eigakan";
				string key = $"movie-uploads/{movieId}/{fileName}";

				var request = new GetPreSignedUrlRequest
				{
					BucketName = bucketName,
					Key = key,
					Expires = DateTime.UtcNow.AddMinutes(30)
				};

				string presignedUrl = _s3Client.GetPreSignedURL(request);

				return new Result<UploadPictureResponse>
				{
					Success = true,
					Data = new UploadPictureResponse
					{
						Url = presignedUrl
					},
					Message = "Get PreSignedUrl Successful!"
				};

			}
			catch (Exception ex)
			{
				//await _logger.LogError(ex, nameof(AwsS3Service));
				return new Result<UploadPictureResponse>
				{
					Success = false,
					Message = ex.Message
				};
			}


		}

		public async Task<Result<UploadPictureResponse>> GetPreFileContract(string userId, string fileName)
        {
            try
            {
                var userIdclaim = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
                var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);

                if (userId != userIdclaim.Value && roleClaim.Value != "ADMIN")
                {
                    return new Result<UploadPictureResponse>
                    {
                        Success = false,
                        Message = "You have not permission here"
                    };
                }

                string bucketName = "file-eigakan";
                string key = $"contracts/{userId}/{fileName}";

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                };

                string presignedUrl = _s3Client.GetPreSignedURL(request);

                return new Result<UploadPictureResponse>
                {
                    Success = true,
                    Data = new UploadPictureResponse
                    {
                        Url = presignedUrl
                    },
                    Message = "Get PreSignedUrl Successful!"
                };

            }
            catch (Exception ex)
            {
                //await _logger.LogError(ex, nameof(AwsS3Service));
                return new Result<UploadPictureResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }


        }
    }
}
