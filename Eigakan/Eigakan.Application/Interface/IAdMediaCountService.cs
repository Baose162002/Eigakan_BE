using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.AdMedia;
using Eigakan.Domain.Response.AdMediaCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
	public interface IAdMediaCountService 
	{
		Task<Result<AdMediaCount>> GetAdMediaCountByAdMediaId(string? adMediaId);
		Task<Result<AdMediaCount>> IncreaseAdMediaCount(AdClickCountCreateRequest adClickCount);
		Task<object> StatisticAdMediaCount(string adMediaId);
		Task<Result<AdMediaCountGetAllResponse>> CreateCountAdMediaAsync(string mediaId);

    }
} 