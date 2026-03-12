using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface ICommentRepository :IGenericRepository<Comment>
    {
        Task<Comment> GetCommentById(string? id);
        Task<bool> DeleteCommentAsync(string? Id);
        Task<List<Comment>> GetList();
        Task<List<Comment>> GetListMovieId(string? movieId);
    }
}
