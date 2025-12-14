using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.Repository
{
    public interface IRepositoryBase<T,TKey>
    {
        IQueryable<T> FindAll();
        T FindById(Guid id);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition);
        Task Create(T entity);
        Task Update(T entity, TKey id);
        Task Delete(T entity);
        //Task DeleteAll();
        Task Save();
    }
}
