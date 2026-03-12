using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class Role
	{
		public string Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }

		[JsonIgnore]
		public ICollection<User>? Users { get; set; }
	}
}
