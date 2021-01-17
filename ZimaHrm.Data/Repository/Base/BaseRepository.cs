using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private DbSet<T> entities;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
            entities = _context.Set<T>();
        }
        public IQueryable<T> All()
        {
            return entities.Where(x => !x.IsDelete);
        }

        public IQueryable<T> All(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = All();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<T, object>(includeProperty);
            }

            return queryable;
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            _context.SaveChanges();
        }

        public T Find(Guid id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public T Find(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.CreatedUtc = DateTime.UtcNow;
            entity.LastModifiedUtc = DateTime.UtcNow;
            entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity, Guid id)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            T exist = _context.Set<T>().Find(id);
            if (exist != null)
            {
                entity.LastModifiedUtc = DateTime.UtcNow;
                _context.Entry(exist).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
            _context.SaveChanges();
        }
    }
}
