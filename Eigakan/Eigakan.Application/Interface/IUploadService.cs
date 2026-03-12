using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IUploadService
	{
		Task<Result<UploadPictureResponse>> UploadPictureUserAsync(IFormFile file);
		Task<Result<UploadPictureResponse>> UploadFileAsync(IFormFile file);
	}
}
