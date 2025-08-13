using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public interface IDataStoreFactory<TContext> where TContext : DbContext
{
    IDataStore<TEntity> CreateStore<TEntity>() where TEntity : class, IEntity;
}