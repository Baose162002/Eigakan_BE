using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eigakan.Domain.Models
{
	public class Contract
	{
		[Key]
		public string Id { get; set; }

		[MaxLength(500)]
		public string? FileUrl { get; set; }
		public DateTime? ContractDate { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public int? Duration { get; set; }
		public decimal? Price { get; set; }

        public string? ExtendRequest { get; set; }
        public string? ExtendStatus { get; set; }
        public string? OriginalContractId { get; set; }

        [MaxLength(255)]
		public string? PublisherName { get; set; }

		[MaxLength(255)]
		public string? DistributorName { get; set; }

		public DateTime? CreateDate { get; set; }
		public DateTime? UpdateDate { get; set; }

		[MaxLength(50)]
		public string? Status { get; set; }

		[MaxLength(1000)]
		public string? ReasonForDenying { get; set; }

		[JsonIgnore]
		[MaxLength(500)]
		public string? SignToken { get; set; }

		public bool? IsSigned { get; set; }

		[MaxLength(50)] 
		public string? UserId { get; set; }
		public User? User { get; set; }

		[MaxLength(50)]
		public string? MovieId { get; set; }
		public Movie? Movie { get; set; }
	}
}
