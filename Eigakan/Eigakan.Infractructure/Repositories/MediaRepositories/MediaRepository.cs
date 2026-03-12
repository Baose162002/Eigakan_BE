using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.MediaRepositories
{
    public class MediaRepository :GenericBase<Media> ,IMediaRepository
    {

        private readonly EigakanDbContext _context;

        public MediaRepository(EigakanDbContext context)
        {
            _context = context;
        }

        public async Task<List<Media>> GetList()
        {
            var contracts = await Get();
            return contracts.ToList();
        }
       
        public async Task<Media> GetMediaById(string id)
        {
            return await GetSingle(
       filter: c => c.Id == id 
   );
        }

        public async Task<bool> DeleteMediaAsync(string? Id)
        {
         

            var media = await _context.Media.FindAsync(Id);
            if (media != null)
            {
                _context.Media.Remove(media);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
