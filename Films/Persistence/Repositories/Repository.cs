using System.Linq.Expressions;
using Films.Core.Domain.Filters;
using Films.Core.Domain.Pagination;
using Films.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Films.Persistence.Repositories;

public abstract class Repository<TContext, TEntity, TFilter> : Repository<TContext, TEntity>, IRepository<TEntity, TFilter> where TContext : DbContext where TEntity : class where TFilter : BaseFilter
{
    protected Repository(TContext context) : base(context)
    {
    }

    public IReadOnlyList<TEntity> GetAll(TFilter filter)
    {
        var query = GetQuery(filter);
        return query.ToList();
    }

    public virtual PaginatedList<TEntity> ToPagedList(TFilter filter)
    {
        var query = GetQuery(filter);
        return ToPagedList(query, filter.PageNo, filter.PageSize);
    }

    protected abstract IQueryable<TEntity> GetQuery(TFilter filter);
}

public abstract class Repository<TContext, TEntity> : IRepository<TEntity> where TEntity : class where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected Repository(TContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }

    public IReadOnlyList<TEntity> GetAll()
    {
        return DbSet.ToList();
    }

    public IReadOnlyList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Where(predicate).ToList();
    }

    public TEntity? Find(params object[] keyValues)
    {
        return DbSet.Find(keyValues);
    }

    public TEntity Single(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Single(predicate);
    }

    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.SingleOrDefault(predicate);
    }

    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.FirstOrDefault(predicate);
    }

    public PaginatedList<TEntity> ToPagedList(Expression<Func<TEntity, bool>> predicate, int pageNo = 1, int pageSize = 10)
    {
        var query = DbSet.Where(predicate);
        return ToPagedList(query, pageNo, pageSize);
    }

    protected PaginatedList<TEntity> ToPagedList(IQueryable<TEntity> query, int pageNo = 1, int pageSize = 10)
    {
        var count = query.Count();
        var pageCount = GetPageCount(count, pageSize);
        var pagedList = query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedList<TEntity>(pagedList, new PaginationInfo(pageNo, pageSize, pageCount, count));
    }

    protected int GetPageCount(int count, int pageSize)
    {
        if (count <= 0) return 0;
        return (int)Math.Ceiling(count / (double)pageSize);
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Any(predicate);
    }

    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbSet.AddRange(entities);
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public bool Remove(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = SingleOrDefault(predicate);
        if (entity == null) return false;

        Remove(entity);
        return true;
    }
}