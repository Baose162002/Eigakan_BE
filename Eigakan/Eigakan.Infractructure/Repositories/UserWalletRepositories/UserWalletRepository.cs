using Eigakan.Application.Interface.IRepository;
using Eigakan.Domain.Models;
using Eigakan.Infractructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.UserWalletRepositories
{
    public class UserWalletRepository : GenericBase<Domain.Models.UserWallet>, IUserWalletRepository
    {
        public async Task<UserWallet> GetUserWalletById(string userId)
        {
            return await GetSingle(filter: c => c.UserId == userId);
        }
        public async Task<UserWallet> GetWalletById(string id)
        {
            return await GetSingle(filter: c => c.Id == id);
        }

    }
}
