using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Movies.Persistence.Infrastructure;

public class DataStore<TEntity>(DbContext dbContext) : IDataStore<TEntity>
    where TEntity : class
{
    private DbSet<TEntity> DbSet => dbContext.Set<TEntity>();
    
    public IQueryable<TEntity> AsQueryable()
    {
        return DbSet.AsQueryable();
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = AsQueryable();

        query = IncludeProperties(query, includeProperties);

        if (predicate != null)
        {
            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = AsQueryable();
        
        query = IncludeProperties(query, includeProperties);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TProjection>> ListAsync<TProjection>(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Expression<Func<TEntity, TProjection>>? projection = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (projection != null)
        {
            IQueryable<TProjection> projectionQuery = query.Select(projection);
            return await projectionQuery.ToListAsync(cancellationToken);
        }

        throw new ArgumentNullException(nameof(projection));
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    private IQueryable<TEntity> IncludeProperties(IQueryable<TEntity> query, params string[]? includeProperties)
    {
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }
        
        return query;
    }
}