using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class User
	{
		public string Id { get; set; }
		[MaxLength(255)]
		public string? FullName { get; set; }
		public bool? Gender { get; set; }
		public string? Birthday { get; set; }
		[MaxLength(1000)]
		public string? Picture { get; set; }
		[MaxLength(100)]
		public string? Email { get; set; }
		public DateTime? CreateDate { get; set; }
		[MaxLength(100)]
		public string? Status { get; set; }

		public string? RoleId { get; set; }
		public Role? Role { get; set; }

		public string? UserRegisterId { get; set; }
		public UserRegister? UserRegister { get; set; }

		public UserWallet? UserWallet { get; set; }

		[JsonIgnore]
		public byte[] PasswordHash { get; set; } = new byte[32];
		[JsonIgnore]
		public byte[] PasswordSalt { get; set; } = new byte[32];
		[JsonIgnore]
		public string? VerificationToken { get; set; }
		[JsonIgnore]
		public DateTime? VerifiedAt { get; set; }
		[JsonIgnore]
		public string? PasswordResetToken { get; set; }
		[JsonIgnore]
		public DateTime? ResetTokenExpirex { get; set; }
		[JsonIgnore]
		public string RefreshToken { get; set; } = string.Empty;
		[JsonIgnore]
		public DateTime? TokenCreated { get; set; }
		[JsonIgnore]
		public DateTime? TokenExpires { get; set; }

		public ICollection<News>? Newss { get; set; }
		public ICollection<MovieHistory>? MovieHistories{ get; set; }
		public ICollection<SubscriptionPurchase>? SubscriptionPurchases { get; set; }
		public ICollection<AdPurchaseTransaction>? AdPurchases { get; set; }
		public ICollection<Contract>? Contracts { get; set; }
		public ICollection<Movie>? Movies { get; set; }
		public ICollection<MovieRating>? MovieRatings { get; set; }
		public ICollection<UserEarning>? UserEarnings{ get; set; }
    }
}
