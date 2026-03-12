using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.Genre
{
    public class CreateGenreRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
