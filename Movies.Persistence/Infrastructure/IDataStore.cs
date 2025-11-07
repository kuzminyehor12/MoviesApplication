using System.Linq.Expressions;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public interface IDataStore<TEntity>
    where TEntity : IEntity
{
    IQueryable<TEntity> AsQueryable(bool tracking = false);
    
    Task<TEntity> FirstAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default);
    
    Task<IList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string[]? includeProperties = null,
        bool tracking = false,
        CancellationToken cancellationToken = default);
    
    Task<IList<TProjection>> ListAsync<TProjection>(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TProjection>>? projection = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool tracking = false,
        CancellationToken cancellationToken = default);
    
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task BulkAddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    Task BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}