using RockyShop.DataAccess.Data;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RockyShop.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private const string includeAllProperties = $"{nameof(OrderDetails.Product)},{nameof(OrderDetails.OrderHeader)}";

        public OrderDetailsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public OrderDetails FirstOrDefaultIncludeAll(Expression<Func<OrderDetails, bool>> filter = null, bool isTracking = true)
        {
            return base.FirstOrDefault(filter, includeAllProperties, isTracking);
        }

        public IEnumerable<OrderDetails> GetAllIncludeAll(Expression<Func<OrderDetails, bool>> filter = null, Func<IQueryable<OrderDetails>, IOrderedQueryable<OrderDetails>> orderBy = null, bool isTracking = true)
        {
            return base.GetAll(filter, orderBy, includeAllProperties, isTracking);
        }
    }
}
