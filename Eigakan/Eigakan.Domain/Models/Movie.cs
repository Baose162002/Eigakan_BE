using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class Movie
	{
		public string? Id { get; set; }
		[MaxLength(255)]
		public string? Title { get; set; }
		[MaxLength(255)]
		public string? OriginName { get; set; }
		[MaxLength(5000)]
		public string? Description { get; set; }
		public string? ReleaseYear { get; set; }
		public int? Duration { get; set; }
		[MaxLength(255)]
		public string? Director { get; set; }
		public string? Script { get; set; }
		[MaxLength(255)]
		public string? Nation { get; set; }
		public double Rating { get; set; }
        public bool? IsContract { get; set; }
		[MaxLength(1000)]
		public string? FileUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
        public DateTime? SubmissionDate { get; set; } // ngày duyệt phim để bắt đầu tạo contract
		public string? ReasonForRejection { get; set; }
		[MaxLength(100)]
		public string? Status { get; set; }
		public string? UserId { get; set; }
		public User? User { get; set; }

		public ICollection<Contract>? contracts { get; set; } 
		public ICollection<News>? Newss { get; set; }
		public ICollection<MovieGenre>? MovieGenres { get; set; }
		public ICollection<MoviePerson>? MoviePersons { get; set; }
		public ICollection<Comment>? Comments { get; set; }
		public ICollection<Media>? Media { get; set; }
		public ICollection<MovieRating>? MovieRatings { get; set; }
		[JsonIgnore]
		public ICollection<MovieHistory>? MovieHistories { get; set; }
        public ICollection<MovieCount>? MovieCounts { get; set; }
		public ICollection<MovieEarning>? MovieEarnings{ get; set; }
    }
}
