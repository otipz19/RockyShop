using System.Linq.Expressions;

namespace RockyShop.DataAccess.Repository.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        public void SaveChanges();

        public void Add(T entity);

        public void Update(T entity);

        public void Remove(T entity);

        public T Find(int id);

        //public T FindAsNoTracking(int id);

        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true);

        public T FirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true);
    }
}
