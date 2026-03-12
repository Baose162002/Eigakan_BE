using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.Movie
{
	public class RejectedMovieRequest
	{
		public string? Id { get; set; }
		public string? ReasonForRejection { get; set; }
	}
}
