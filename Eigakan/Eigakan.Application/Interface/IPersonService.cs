using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Person;
using Eigakan.Domain.Response.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface IPersonService
    {
        Task<Result<List<PersonListResponse>>> GetList(int page, int pagesize, string? name, bool? gender);
        Task<Result<Person>> CreatePerson(PersonCreateRequest personRequest);
        Task<Result<PersonReturnMovieListResponse>> GetPersonById(string? id);
		Task<Result<Person>> UpdatePerson(string? id, PersonCreateRequest request);
        Task<Result<Person>> DeletePerson(string? id);
        //Task<Result<PersonReturnMovieListResponse>> GetPersonByIdTEST(string? id);

    }
}
