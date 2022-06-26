using eAuction.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.DataAccess.Repositories.ProductRepositories
{
    public interface IProductRepository
    {
        Products GetProduct(string productId);
        void Create(Products obj);
        bool Delete(string id);
        List<Products> GetProductList();
    }
}
