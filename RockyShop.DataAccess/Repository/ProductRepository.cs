using RockyShop.DataAccess.Data;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private const string includeAllProperties = $"{nameof(Product.Category)},{nameof(Product.ApplicationType)}";

        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {
            
        }

        public Product FirstOrDefaultIncludeAll(Expression<Func<Product, bool>> filter = null, bool isTracking = true)
        {
            return base.FirstOrDefault(filter, includeAllProperties, isTracking);
        }

        public IEnumerable<Product> GetAllIncludeAll(Expression<Func<Product, bool>> filter = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, bool isTracking = true)
        {
            return base.GetAll(filter, orderBy, includeAllProperties, isTracking);
        }
    }
}
