using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class Comment
	{
		public string Id { get; set; }
		public string? Content { get; set; }
		public string? CreateBy { get; set; }
		public DateTime? CreateDate { get; set; }
		public string? MovieId { get; set; }
		[JsonIgnore]
		public Movie? Movie { get; set; }
	}
}
