using System.Linq.Expressions;

namespace Movies.Persistence.Infrastructure;

public interface IDataStore<TEntity>
    where TEntity : class
{
    IQueryable<TEntity> AsQueryable();
    
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string[]? includeProperties = null, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TProjection>> ListAsync<TProjection>(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TProjection>>? projection = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}