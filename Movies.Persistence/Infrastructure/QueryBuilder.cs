using System.Linq.Expressions;

namespace Movies.Persistence.Infrastructure;

public class QueryBuilder<TEntity>(IQueryable<TEntity> queryable)
    where TEntity : class
{
    private IQueryable<TEntity> _queryable = queryable;

    public QueryBuilder<TEntity> Filter(Expression<Func<TEntity, bool>> predicate)
    {
        _queryable = _queryable.Where(predicate);
        return this;
    }

    public QueryBuilder<TEntity> Take(int limit)
    {
        _queryable = _queryable.Take(limit);
        return this;
    }

    public QueryBuilder<TEntity> Skip(int limit)
    {
        _queryable = _queryable.Skip(limit);
        return this;
    }

    public TEntity First(Expression<Func<TEntity, bool>> predicate)
    {
        return _queryable.First(predicate);
    }

    public QueryBuilder<TProjection> Map<TProjection>(Expression<Func<TEntity, TProjection>> selector) 
        where TProjection : class
    {
        return new QueryBuilder<TProjection>(_queryable.Select(selector));
    }

    public IQueryable<TEntity> Query()
    {
        return _queryable;
    }
}