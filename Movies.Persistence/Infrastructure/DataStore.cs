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

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}