using Eigakan.Domain.Response.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Response.Genre
{
    public class GenreReturnMovieListResponse
    {

        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public List<GenreMovieList> movieList { get; set; }
    }
}
