using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.Person
{
    public class PersonCreateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Job { get; set; }
        public bool? Gender { get; set; }
        public string? Birthday { get; set; }
        public string? Picture { get; set; }
    }
}
