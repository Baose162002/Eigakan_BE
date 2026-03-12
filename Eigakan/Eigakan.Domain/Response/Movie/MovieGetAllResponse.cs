using Eigakan.Domain.Response.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.Movie
{
    public class MovieGetAllResponse
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public string? OriginName { get; set; }
        public string? Description { get; set; }
        public int? ViewCount { get; set; }
        public string? ReleaseYear { get; set; }
        public int? Duration { get; set; }
        public string? Director { get; set; }
        public string? Script { get; set; }
        public string? Nation { get; set; }
        public double? Rating { get; set; }
        public bool? IsContract { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? SubmissionDate { get; set; } 
        public string? ReasonForRejection { get; set; }
		public bool IsFilmVipOrTrailer { get; set; }

		public double? UserRating { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public ICollection<MediaShortRespone>? Medias { get; set; }
    }
}
