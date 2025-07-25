using System.Linq.Expressions;

namespace Movies.Persistence.Infrastructure;

public interface IDataStore<TEntity>
    where TEntity : class
{
    IQueryable<TEntity> AsQueryable();
    
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}