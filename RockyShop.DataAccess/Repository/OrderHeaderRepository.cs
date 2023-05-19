using RockyShop.DataAccess.Data;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private const string includeUserProperty = $"{nameof(OrderHeader.CreatedByUser)}";

        public OrderHeaderRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public OrderHeader FirstOrDefaultIncludeUser(Expression<Func<OrderHeader, bool>> filter = null, bool isTracking = true)
        {
            return base.FirstOrDefault(filter, includeUserProperty, isTracking);
        }
    }
}
