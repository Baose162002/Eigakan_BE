using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Models
{
	public class MovieCount
	{
        public string? Id { get; set; }
        public DateOnly? ViewDate { get; set; }
        public int? ViewCount { get; set; }
        public string? MovieId { get; set; }
        public Movie? Movies { get; set; }

    }
}
