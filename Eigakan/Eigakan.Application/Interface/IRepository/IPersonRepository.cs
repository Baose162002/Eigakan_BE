using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        Task<List<Domain.Models.Person>> GetList(int pageIndex, int pageSize, string? name, bool? gender);
        Task<Person> GetPersonById(string id);
        Task<List<string>> GetListPersonById(List<string>? persons);

        Task<Person> GetPersById(string? id);

        Task<bool> DeletePersonAsync(string? Id);
    }
}
