using RockyShop.Model.Models;
using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository.Interfaces
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        public OrderHeader FirstOrDefaultIncludeUser(Expression<Func<OrderHeader, bool>> filter = null, bool isTracking = true);
    }
}
