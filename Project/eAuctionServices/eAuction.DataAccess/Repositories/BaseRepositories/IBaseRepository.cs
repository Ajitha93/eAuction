using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.DataAccess.Repositories.BaseRepositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Create(TEntity obj);
    }
}
