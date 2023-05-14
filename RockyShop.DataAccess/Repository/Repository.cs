using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RockyShop.DataAccess.Data;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;

namespace RockyShop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T>
        where T: class
    {
        public const char PropertySeparator = ',';

        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public T Find(int id)
        {
            return _dbSet.Find(id);
        }

        //public T FindAsNoTracking(int id)
        //{
        //    PropertyInfo idProp = typeof(T).GetProperty(name: "Id", returnType: typeof(int));
        //    if(idProp != null)
        //    {
        //        return _dbSet.AsNoTracking()
        //            .FirstOrDefault(e => (int) idProp.GetValue(e) == id);
        //    }
        //    return null;
        //}

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public T FirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (!includeProperties.IsNullOrEmpty())
            {
                foreach (var prop in includeProperties
                    .Split(PropertySeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(prop);
                }
            }
            if (!isTracking)
                query = query.AsNoTracking();
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (orderBy != null)
                query = orderBy(query);
            if (!includeProperties.IsNullOrEmpty())
            {
                foreach (var prop in includeProperties
                    .Split(PropertySeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(prop);
                }
            }
            if (!isTracking)
                query = query.AsNoTracking();
            return query.ToList();
        }
    }
}
