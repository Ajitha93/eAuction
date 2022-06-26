using eAuction.DataAccess.DBContext;
using eAuction.DataAccess.Repositories.BaseRepositories;
using eAuction.Models.EntityModels;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.DataAccess.Repositories.ProductRepositories
{
    public class ProductRepository : BaseRepository<Products>, IProductRepository
    {
        public ProductRepository(IMongoDBContext context) : base(context)
        {
        }

        public Products GetProduct(string productId)
        {
            var all = _dbCollection.Find(Builders<Products>.Filter.Empty, null);
            var product = all.ToList()
                ?.Where(x => x.ProductId == productId)
                ?.FirstOrDefault();
            return product;
        }      

        public bool Delete(string id)
        {
            _dbCollection.Find(Builders<Products>.Filter.Empty, null);
            _dbCollection.DeleteOne(x => x.ProductId == id);
            return true;
        }

        public List<Products> GetProductList()
        {
            var products = _dbCollection.Find(Builders<Products>.Filter.Empty, null)?.ToList();           
            return products;
        }

    }
}
