using System.Linq.Expressions;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public interface IDataStore<TEntity>
    where TEntity : IEntity
{
    IQueryable<TEntity> AsQueryable();
    
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
    
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task BulkAddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task<TEntity> GetOrAddAsync(Expression<Func<TEntity, bool>> getter, Func<TEntity> valueFactory, CancellationToken cancellationToken = default);
}