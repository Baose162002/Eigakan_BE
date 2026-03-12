using DocumentFormat.OpenXml.Spreadsheet;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MovieCountRepositories
{
	public class MovieCountRepository : GenericBase<MovieCount>, IMovieCountRepository
	{
		public async Task<MovieCount> GetMovieCountByMovieId(string movieId)
		{
			return (await GetSingle(u => u.MovieId == movieId));
		}

		public async Task<MovieCount> CheckCountByMovieDate(string movieId, DateOnly dateTime)
		{
			return (await GetSingle(u => u.MovieId == movieId && u.ViewDate == dateTime));
		}

		public async Task<List<MovieCount>> GetAllMovieCountByMovieId(string movieId)
		{
			return (await Get(u => u.MovieId == movieId)).ToList();
		}
	}
}
