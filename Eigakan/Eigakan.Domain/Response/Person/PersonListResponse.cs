using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.Person
{
    public class PersonListResponse
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Job { get; set; }
        public bool? Gender { get; set; }
        public string? Birthday { get; set; }
        public string? Picture { get; set; }

        [JsonIgnore]
        public ICollection<MoviePerson>? MoviePersons { get; set; }
    }
}
