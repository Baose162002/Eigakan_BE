using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.CommentRepositories
{
    public class CommentRepository :GenericBase<Comment>,ICommentRepository
    {
        private readonly EigakanDbContext _context;

        public CommentRepository(EigakanDbContext context)
        {
            _context = context;
        }
        public async Task<List<Comment>> GetList()
        {
            var comments = await _context.Comments
            .OrderByDescending(c => c.CreateDate)
            
            .ToListAsync();
            return comments;
        }
        public async Task<List<Comment>> GetListMovieId(string? movieId)
        {
            var comments = await _context.Comments
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreateDate)
                
                .ToListAsync();

            return comments;
        }

        public async Task<Comment> GetCommentById(string? id)
        {
            return await GetSingle(
             filter: c => c.Id == id
             );
        }
        public async Task<bool> DeleteCommentAsync(string? Id)
        {


            var media = await _context.Comments.FindAsync(Id);
            if (media != null)
            {
                _context.Comments.Remove(media);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
