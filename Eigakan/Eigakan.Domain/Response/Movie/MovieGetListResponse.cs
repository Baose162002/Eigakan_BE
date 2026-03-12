using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.Movie
{
    public class MovieGetListResponse
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
        public double? UserRating { get; set; }
        public bool? IsContract { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? SubmissionDate { get; set; } // ngày duyệt phim để bắt đầu tạo contract
        public string? ReasonForRejection { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
		public ICollection<Contract>? contracts { get; set; }

		[JsonIgnore]
        public ICollection<MovieGenre>? MovieGenres { get; set; }
        [JsonIgnore]
        public ICollection<MoviePerson>? MoviePersons { get; set; }
       
        public ICollection<Eigakan.Domain.Models.Media>? Medias { get; set; }
        [JsonIgnore]
        public ICollection<MovieRating>? MovieRatings { get; set; }
        public string? GenreNames { get; set; }
        public List<Domain.Models.Person>? Person { get; set; }  
    }
}
