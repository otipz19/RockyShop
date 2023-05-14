using RockyShop.Model.Models;
using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        public IEnumerable<Product> GetAllIncludeAll(
             Expression<Func<Product, bool>> filter = null,
             Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
             bool isTracking = true);

        public Product FirstOrDefaultIncludeAll(
            Expression<Func<Product, bool>> filter = null,
            bool isTracking = true);
    }
}
