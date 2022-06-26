using eAuction.DataAccess.DBContext;
using eAuction.DataAccess.Repositories.BaseRepositories;
using eAuction.Models.EntityModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.DataAccess.Repositories.BuyerRepositories
{
    public class BuyerRepository : BaseRepository<Buyers>, IBuyerRepository
    {
        public BuyerRepository(IMongoDBContext context) : base(context)
        {

        }

        public List<Buyers> GetBuyer(string productId, string email = null)
        {
            var all = _dbCollection.Find(Builders<Buyers>.Filter.Empty, null);
            var buyersLst = all.ToList()
                ?.Where(x => x.ProductId == productId && (email==null || x.Email==email))
                ?.ToList();
            return buyersLst;
        }


        public bool Update(string buyerId,Buyers updatedBuyer)
        {
            _dbCollection.ReplaceOne(x => x.BuyerId == buyerId, updatedBuyer);
            return true;
        }
    }
}
