using Eigakan.Domain.Models;
using Eigakan.Domain.Response.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.MovieHistory
{
	public class MovieHistoryResponse
	{
		public string Id { get; set; }
        public string? UserId { get; set; }
        public DateTime CreateDate { get; set; }
		public MovieResponse? Movies { get; set; }
	}

	public class MovieResponse
	{
        public string? Id { get; set; }
        public string? Title { get; set; }
		public string? ReleaseYear { get; set; }
		public int? Duration { get; set; }
		public ICollection<MediaShortRespone>? Medias { get; set; }
	}
}
