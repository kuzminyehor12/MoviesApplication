using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Npgsql;

namespace Movies.Persistence.Infrastructure;

public class DataStore<TEntity>(DbContext dbContext) : IDataStore<TEntity>
    where TEntity : class, IEntity
{
    private DbSet<TEntity> DbSet => dbContext.Set<TEntity>();
    
    private static readonly SemaphoreSlim _semaphore = new(1, 1);   
    
    public IQueryable<TEntity> AsQueryable()
    {
        return DbSet.AsQueryable();
    }

    public async Task<TEntity> FirstAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        string[]? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = AsQueryable().AsNoTracking();

        query = IncludeProperties(query, includeProperties);

        if (predicate != null)
        {
            return await query.FirstAsync(predicate, cancellationToken);
        }

        return await query.FirstAsync(cancellationToken);
    }

    public async Task<IList<TEntity>>  ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        string[]? includeProperties = null,
        bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }
        
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

    public async Task<IList<TProjection>> ListAsync<TProjection>(
        Expression<Func<TEntity, bool>>? predicate = null, 
        Expression<Func<TEntity, TProjection>>? projection = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = AsQueryable();
        
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

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

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task BulkAddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        finally 
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> GetOrAddAsync(Expression<Func<TEntity, bool>> getter, Func<TEntity> valueFactory, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            bool exists = await ExistsAsync(getter, cancellationToken);

            if (exists)
            {
                return await FirstAsync(getter, cancellationToken: cancellationToken);
            }

            return await AddAsync(valueFactory(), cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
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