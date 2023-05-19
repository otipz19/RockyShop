using RockyShop.Model.Models;
using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository.Interfaces
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        public IEnumerable<OrderDetails> GetAllIncludeAll(
             Expression<Func<OrderDetails, bool>> filter = null,
             Func<IQueryable<OrderDetails>, IOrderedQueryable<OrderDetails>> orderBy = null,
             bool isTracking = true);

        public OrderDetails FirstOrDefaultIncludeAll(
            Expression<Func<OrderDetails, bool>> filter = null,
            bool isTracking = true);
    }
}
