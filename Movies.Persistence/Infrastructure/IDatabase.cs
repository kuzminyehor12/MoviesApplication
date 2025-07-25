using Microsoft.EntityFrameworkCore;

namespace Movies.Persistence.Infrastructure;

public interface IDatabase<TContext>
    where TContext : DbContext
{
    IDataStore<TEntity> Store<TEntity>() where TEntity : class;
    
    Task CreateDatabaseAsync(CancellationToken cancellationToken = default);
    
    Task DropDatabaseAsync(CancellationToken cancellationToken = default);
}