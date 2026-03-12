using Eigakan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Interface.IRepository
{
    public interface IMediaRepository:IGenericRepository<Media>
    {
        Task<List<Media>> GetList();
        Task<Media> GetMediaById(string id);
        Task<bool> DeleteMediaAsync(string? Id);
    }
}
