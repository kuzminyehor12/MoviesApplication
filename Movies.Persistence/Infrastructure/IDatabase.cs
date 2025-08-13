using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public interface IDatabase<TContext>
    where TContext : DbContext
{
    IDataStore<TEntity> Store<TEntity>() where TEntity : class, IEntity;
    
    Task CreateDatabaseAsync(CancellationToken cancellationToken = default);
    
    Task DropDatabaseAsync(CancellationToken cancellationToken = default);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}