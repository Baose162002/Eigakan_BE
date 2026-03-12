using DocumentFormat.OpenXml.Wordprocessing;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace Eigakan.Infractructure.Repositories.GerneRepositories
{

    public class GerneRepository : GenericBase<Genre>, IGenreRepository
    {

        private readonly EigakanDbContext _context;

        public GerneRepository(EigakanDbContext context)
        {
            _context = context;
        }
       

        public async Task<List<Genre>> GetList()
        {
            return (await Get()).ToList();
        }

        public async Task<int> CheckName(string? names)
        {

            var genre = _context.Genres
                  .Where(g => names == g.Name);

            if (genre.Any()) { return 1; }
            return 0;
        }
        public async Task<Genre> GetGenreById(string? id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return await _context.Genres
                         .Include(g => g.MovieGenres)
                             .ThenInclude(mg => mg.Movie)
                                 .ThenInclude(m => m.Media) 
                         .Include(g => g.MovieGenres)
                             .ThenInclude(mg => mg.Movie)
                                 .ThenInclude(m => m.MoviePersons).ThenInclude(m => m.Person) 
                         .FirstOrDefaultAsync(g => g.Id == id);


        }


        public Task<Genre> GetListMovieByGenreId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetListGenreById(List<string>? genres)
        {
            if (genres == null || !genres.Any())
                return new List<string>();
            var movieGenres = await _context.MovieGenres.Where(mg => mg.GenreId == genres.ToString()).ToListAsync();
            return await _context.Genres
                .Where(g => genres.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();
        }
        public async Task<bool> DeleteGenreAsync(string? Id)
        {
            var movieGenres = _context.MovieGenres.Where(mg => mg.GenreId == Id);
            _context.MovieGenres.RemoveRange(movieGenres);

            var genre = await _context.Genres.FindAsync(Id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

    }
}
