using System.Linq.Expressions;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public interface IDataStore<T>
    where T : BaseEntity
{
    Task AddAsync(T entity);
    
    Task AddRangeAsync(IEnumerable<T> entities);
    
    Task DeleteAsync(T entity);
    
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate);
}