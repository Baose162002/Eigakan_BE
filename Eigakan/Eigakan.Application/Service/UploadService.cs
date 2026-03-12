using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eigakan.Application.Interface;
using Eigakan.Domain.Request.ContractRequest;
using Firebase.Storage;

namespace Eigakan.Application.Service
{
	public class UploadService : IUploadService
	{
		private readonly Cloudinary _cloudinary;

		public UploadService(Cloudinary cloudinary) 
		{
			_cloudinary = cloudinary;
		}

		public async Task<Result<UploadPictureResponse>> UploadPictureUserAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)

				return new Result<UploadPictureResponse>
				{
					Success = false,
					Message = "File not exists"
				};

			// Upload image to Cloudinary
			var uploadParams = new ImageUploadParams()
			{
				File = new FileDescription(file.FileName, file.OpenReadStream())
			};

			var uploadResult = await _cloudinary.UploadAsync(uploadParams);

			var uploadData = new UploadPictureResponse
			{
				Url = uploadResult.SecureUrl.ToString(),
				PublicId = uploadResult.PublicId
			};

			return new Result<UploadPictureResponse>
			{
				Success = true,
				Data = uploadData,
				Message = "Upload Successful!"
			};

		}

		public async Task<Result<UploadPictureResponse>> UploadFileAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)

				return new Result<UploadPictureResponse>
				{
					Success = false,
					Message = "File not exists"
				};

			string fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

			using var memoryStream = new MemoryStream();
			await file.CopyToAsync(memoryStream);
			byte[] fileBytes = memoryStream.ToArray();

			try
			{
				using var uploadStream = new MemoryStream(fileBytes);
				var task = new FirebaseStorage("free-fb08a.appspot.com")
					.Child("contracts")
					.Child(fileName)
					.PutAsync(uploadStream);

				string downloadUrl = await task;

				var uploadData = new UploadPictureResponse
				{
					Url = downloadUrl
				};

				return new Result<UploadPictureResponse>
				{
					Success = true,
					Data = uploadData,
					Message = "Upload Successful!"
				};
			}
			catch (Exception ex)
			{
				return new Result<UploadPictureResponse>
				{
					Success = false,
					Message = $"Upload failed: {ex.Message}"
				};
			}
		}

	}
}
