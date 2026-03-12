using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class Media
	{
		public string Id { get; set; }
		[MaxLength(100)]
		public string? Name { get; set; }
		[MaxLength(1000)]
		public string? Url { get; set; }
		[MaxLength(255)]
		public string? Type { get; set; }
		public DateTime? CreateDate { get; set; }

		public string? MovieId { get; set; }
		[JsonIgnore]
		public Movie? Movie { get; set; }
	}
}
