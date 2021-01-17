using System;
using System.Linq;
using System.Linq.Expressions;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IQueryable<T> All();
        IQueryable<T> All(params Expression<Func<T, object>>[] includeProperties);
        T Find(Guid id);
        T Find(Guid id, params Expression<Func<T, object>>[] includeProperties);
        void Insert(T entity);
        void Update(T entity, Guid id);
        void Delete(T entity);


    }
}
