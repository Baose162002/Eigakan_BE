using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class MovieHistory
	{
		public string Id { get; set; }
		public DateTime CreateDate { get; set; }
		public string? UserId { get; set; }
		public User? User { get; set; }
		public string? MovieId { get; set; }
		public Movie? Movie { get; set; }
	}
}
