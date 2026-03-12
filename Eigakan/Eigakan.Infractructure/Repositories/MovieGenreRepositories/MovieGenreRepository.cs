using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MovieGenreRepositories
{
    public class MovieGenreRepository :GenericBase<MovieGenre>,IMovieGenreRepository
    {

        private readonly EigakanDbContext _context;

        public MovieGenreRepository(EigakanDbContext context)
        {
            _context = context;
        }
       

    }
}
