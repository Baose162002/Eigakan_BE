using Eigakan.Domain.Request.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.Movie
{
    public class CreateMovieRequest
    {
        public string? Title { get; set; }
        public string? OriginName { get; set; }
        public string? Description { get; set; }
        public string? ReleaseYear { get; set; }
        public int? Duration { get; set; }
        public string? Director { get; set; }
        public string? Script { get; set; }
        public string? Nation { get; set; }
        public bool? IsContract { get; set; }
        public string? FileUrl { get; set; }
        public List<string>? Genres { get; set; }
        public List<string>? Persons { get; set; }
        public List<MediaMovieCreateRequest>? Medias { get; set; }
    }
}
