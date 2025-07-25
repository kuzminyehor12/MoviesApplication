namespace Movies.Persistence.Infrastructure;

public interface IDataStoreFactory
{
    IDataStore<TEntity> CreateStore<TEntity>()
        where TEntity : class;
}