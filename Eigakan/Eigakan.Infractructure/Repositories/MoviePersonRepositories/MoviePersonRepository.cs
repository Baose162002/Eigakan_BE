using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MoviePersonRepositories
{
    public class MoviePersonRepository :GenericBase<MoviePerson> ,IMoviePersonRepository
    {
    }
}
