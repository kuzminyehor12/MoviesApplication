using Microsoft.EntityFrameworkCore;

namespace Movies.Persistence.Infrastructure;

public class DataStoreFactory(DbContext dbContext) : IDataStoreFactory
{
    public IDataStore<TEntity> CreateStore<TEntity>() where TEntity : class
    {
        return new DataStore<TEntity>(dbContext);
    }
}