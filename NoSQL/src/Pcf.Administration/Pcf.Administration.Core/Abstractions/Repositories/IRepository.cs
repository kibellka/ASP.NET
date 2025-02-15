using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pcf.Administration.Core.Domain;

namespace Pcf.Administration.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        
        Task<T> GetByIdAsync(string id);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        IEnumerable<T> FilterBy(Expression<Func<T, bool>> filterExpression);

        IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, TProjected>> projectionExpression);
    }
}