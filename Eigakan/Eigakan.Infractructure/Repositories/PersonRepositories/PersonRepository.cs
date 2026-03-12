using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;

namespace Eigakan.Infractructure.Repositories.PersonRepositories
{
    public class PersonRepository :GenericBase<Person>,IPersonRepository
    {
        private readonly EigakanDbContext _context;

        public PersonRepository(EigakanDbContext context)
        {
            _context = context;
        }


        public async Task<List<Person>> GetList(int pageIndex, int pageSize, string? name, bool? gender)
        {
            return (await Get(
                filter: q => (gender == null || q.Gender == gender) && 
                             (string.IsNullOrEmpty(name) || q.Name.Contains(name)), 
                pageIndex: pageIndex,
                pageSize: pageSize)).ToList();
        }

        public async Task<Person> GetPersonById(string id)
        {
            return await GetSingle(u => u.Id.Equals(id));
        }
        public async Task<List<string>> GetListPersonById(List<string>? persons)
        {
            if (persons == null || !persons.Any())
                return new List<string>();

            return await _context.Persons
                .Where(g => persons.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();
        }

        public async Task<Person> GetPersById(string? id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return await _context.Persons.
                Include(g=>g.MoviePersons)
                .ThenInclude(g=>g.Movie)
                .ThenInclude(g => g.MovieGenres)
                .ThenInclude(g => g.Genre)
              .Include(g => g.MoviePersons)
              .ThenInclude(mg => mg.Movie)
         .ThenInclude(m => m.Media)
                .FirstOrDefaultAsync(g => g.Id == id);

        }

        public async Task<bool> DeletePersonAsync(string? Id)
        {
            var moviePerson = _context.MoviePersons.Where(mg => mg.PersonId == Id);
            _context.MoviePersons.RemoveRange(moviePerson);

            var person = await _context.Persons.FindAsync(Id);
            if (person != null)
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
