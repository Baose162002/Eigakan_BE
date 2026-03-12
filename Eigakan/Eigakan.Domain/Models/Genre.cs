using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class Genre
	{
		public string Id { get; set; }
		[MaxLength(100)]
		public string? Name { get; set; }
		[MaxLength(500)]
		public string? Description { get; set; }
		[JsonIgnore]
		public ICollection<MovieGenre>? MovieGenres { get; set; }
	}
}
