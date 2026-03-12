using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.Comment
{
    public class CommentCreateRequest
    {

        public string? Content { get; set; }
		public string? MovieId { get; set; }
    }
}
