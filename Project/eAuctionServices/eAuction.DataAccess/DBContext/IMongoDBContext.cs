using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.DataAccess.DBContext
{
    public interface IMongoDBContext
    {
        IMongoCollection<Products> GetCollection<Products>(string name);
    }
}
