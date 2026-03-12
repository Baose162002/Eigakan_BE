
using Eigakan.Application.Shared.Response;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface
{
    public interface ICommentService
    {
        Task<Result<Comment>> Delete(string? id);
        Task<Result<Comment>> Create(CommentCreateRequest request);
        Task<Result<Comment>> Update(string? id, CommentUpdateRequest request);
        Task<Result<Comment>> GetCommentById(string? id);
    }
}
