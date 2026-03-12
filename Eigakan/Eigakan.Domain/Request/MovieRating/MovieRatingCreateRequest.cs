using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Request.MovieRating
{
    public class MovieRatingCreateRequest
    {
        [Range(1, 5, ErrorMessage = "Only integer 1 - 5!!!")]
        public int? Stars { get; set; }       
        public string? MovieId { get; set; }
    }
}
