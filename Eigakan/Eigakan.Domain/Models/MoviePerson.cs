using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class MoviePerson
	{
		
		public string? MovieId { get; set; }
        [JsonIgnore]
        public Movie? Movie { get; set; }
		public string? PersonId { get; set; }
        [JsonIgnore]
        public Person? Person { get; set; }
	}
}
