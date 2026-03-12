using AutoMapper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Genre;
using Eigakan.Domain.Request.Person;
using Eigakan.Domain.Response.Genre;
using Eigakan.Domain.Response.Movie;
using Eigakan.Domain.Response.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Service
{
	public class PersonService : IPersonService
	{
		private readonly IMapper _mapper;
		private readonly IPersonRepository _personRepository;
		private readonly Logger _logger;
		public PersonService(IPersonRepository personRepository, IMapper mapper, Logger logger)
		{
			_mapper = mapper;
			_personRepository = personRepository;
			_logger = logger;
		}

		public async Task<Result<Person>> CreatePerson(PersonCreateRequest personRequest)
		{
			try
			{
				var newPer = new Person
				{
					Id = Guid.NewGuid().ToString(),
					Description = personRequest.Description,
					Name = personRequest.Name,
					Birthday = personRequest.Birthday,
					Gender = personRequest.Gender,
					Job = personRequest.Job,
					Picture = personRequest.Picture,
				};
				await _personRepository.Insert(newPer);
				return new Result<Person>
				{ Success = true, Message = "success", Data = newPer };
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(PersonService));
				return new Result<Person> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<List<PersonListResponse>>> GetList(int page, int pagesize,string? name, bool? gender)
		{
			try
			{

				var personlist = await _personRepository.GetList(page, pagesize,name,gender);
				return new Result<List<PersonListResponse>>
				{
					Success = true,
					Data = _mapper.Map<List<PersonListResponse>>(personlist),
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(PersonService));
				return new Result<List<PersonListResponse>> { Success = false, Message = ex.Message };
			}
		}

        public async Task<Result<PersonReturnMovieListResponse>> GetPersonById(string? id)
        {
            try
            {
                var person = await _personRepository.GetPersById(id);
                if (person == null)
                {
                    return new Result<PersonReturnMovieListResponse>
                    {
                        Success = false,
                        Message = "Not found"
                    };
                }

                var per = new PersonReturnMovieListResponse
                {
                    Id = person.Id,
                    Birthday = person.Birthday,
                    Description = person.Description,
                    Gender = person.Gender,
                    Job = person.Job,
                    Name = person.Name,
                    Picture = person.Picture,
                    movieList = person.MoviePersons?
                        .Where(mp => mp.Movie != null && mp.Movie.Status == "ACTIVE") 
                        .Select(mp => new GenreMovieList
                        {
                            Id = mp.Movie.Id,
                            Title = mp.Movie.Title,
                            OriginName = mp.Movie.OriginName,
                            Medias = mp.Movie.Media?
                                .Where(media => media.Type == "POSTER")
                                .Select(media => media.Url)
                                .FirstOrDefault()
                        })
                        .ToList() ?? new List<GenreMovieList>()
                };

                return new Result<PersonReturnMovieListResponse>
                {
                    Success = true,
                    Data = per,
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError(ex, nameof(PersonService));
                return new Result<PersonReturnMovieListResponse> { Success = false, Message = ex.Message };
            }
        }

        public async Task<Result<Person>> UpdatePerson(string? id, PersonCreateRequest request)
		{
			try
			{

				var person = await _personRepository.GetPersById(id);

				if (person == null)
				{
					return new Result<Person>
					{
						Success = false,
						Message = "Not found",
						Data = new Person(),
					};
				}
				person.Birthday = request.Birthday;
				person.Gender = request.Gender;
				person.Name = request.Name;
				person.Description = request.Description;
				person.Job = request.Job;
				person.Picture = request.Picture;

				await _personRepository.Update(person);
				return new Result<Person>
				{
					Success = true,
					Data = person,
				};
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(PersonService));
				return new Result<Person> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<Person>> DeletePerson(string? id)
		{
			try
			{

				var genre = await _personRepository.GetPersById(id);
				if (genre == null)
				{
					return new Result<Person>
					{
						Success = false,
						Message = "Not found",

					};
				}
				if (await _personRepository.DeletePersonAsync(id))
				{

					return new Result<Person>
					{
						Success = true,
						Message = "Delete success"
					};
				}
				else
				{
					return new Result<Person>
					{
						Success = false,
						Message = "Delete fail"
					};
				}
			}
			catch (Exception ex)
			{
				await _logger.LogError(ex, nameof(PersonService));
				return new Result<Person> { Success = false, Message = ex.Message };
			}
		}
	}
}
