using Eigakan.Domain.Models;
using Eigakan.Domain.Response.ContractResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.Movie
{
    public class MovieGetById
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
        public DateTime? SubmissionDate { get; set; } // ngày duyệt phim để bắt đầu tạo contract
        public string? ReasonForRejection { get; set; }

        public double? UserRating { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }

        [JsonIgnore]
        public ICollection<MovieGenre>? MovieGenres { get; set; }
        [JsonIgnore]
        public ICollection<MoviePerson>? MoviePersons { get; set; }

		public ICollection<ContractGetName>? contracts { get; set; }
		public string? GenreNames { get; set; }
        public List<Domain.Models.Person>? Person { get; set; }
        public List<Domain.Models.Comment>? Comments { get; set; }
        public ICollection<Eigakan.Domain.Models.Media>? Medias { get; set; }
    }
}
