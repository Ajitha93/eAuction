using eAuction.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.DataAccess.Repositories.BuyerRepositories
{
    public interface IBuyerRepository
    {
        List<Buyers> GetBuyer(string productId, string email = null);
        void Create(Buyers obj);
        bool Update(string buyerId, Buyers updatedBuyer);
    }
}
