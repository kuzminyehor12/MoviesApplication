using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public class DataStoreFactory<TContext>(TContext dbContext) : IDataStoreFactory<TContext>
    where TContext : DbContext
{
    public IDataStore<TEntity> CreateStore<TEntity>() where TEntity : class, IEntity
    {
        return new DataStore<TEntity>(dbContext);
    }
}