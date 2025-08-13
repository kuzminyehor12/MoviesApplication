using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public class Database<TContext>(
    TContext dbContext,
    IDataStoreFactory<TContext> dataStoreFactory) 
    : IDatabase<TContext> where TContext : DbContext
{
    private readonly IDictionary<string, object> _cachedStores = new ConcurrentDictionary<string, object>();
    
    public IDataStore<TEntity> Store<TEntity>() where TEntity : class, IEntity
    {
        string typeName = typeof(TEntity).Name;
        
        if (_cachedStores.TryGetValue(typeName, out var cachedStore))
        {
            return cachedStore as IDataStore<TEntity> ?? throw new InvalidCastException($"Cannot use cached object as {typeof(IDataStore<TEntity>).Name})");
        }
        
        var store = dataStoreFactory.CreateStore<TEntity>(); 
        
        _cachedStores.Add(typeName, store);
        
        return store;
    }

    public async Task CreateDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task DropDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}