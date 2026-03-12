using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class Person
	{
		public string Id { get; set; }
		[MaxLength(255)]
		public string? Name { get; set; }
		[MaxLength(1000)]
		public string? Description { get; set; }
		[MaxLength(100)]
		public string? Job { get; set; }
		[MaxLength(1000)]
		public string? Picture { get; set; }
        public bool? Gender { get; set; }
		public string? Birthday { get; set; }
		[JsonIgnore]
		public ICollection<MoviePerson>? MoviePersons { get; set; }
	}
}
