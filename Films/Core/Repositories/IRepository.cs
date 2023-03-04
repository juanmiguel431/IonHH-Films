using System.Linq.Expressions;
using Films.Core.Domain.Filters;
using Films.Core.Domain.Pagination;

namespace Films.Core.Repositories;

public interface IRepository<TE, in TF> : IRepository<TE> where TE : class where TF : BaseFilter
{
    IReadOnlyList<TE> GetAll(TF filter);
    PaginatedList<TE> ToPagedList(TF filter);
}

public interface IRepository<T> where T : class
{
    IReadOnlyList<T> GetAll();
    IReadOnlyList<T> Find(Expression<Func<T, bool>> predicate);
    PaginatedList<T> ToPagedList(Expression<Func<T, bool>> predicate, int pageNo = 1, int pageSize = 10);
    T? Find(params object[] keyValues);
    T Single(Expression<Func<T, bool>> predicate);
    T? SingleOrDefault(Expression<Func<T, bool>> predicate);
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);
    public bool Any(Expression<Func<T, bool>> predicate);

    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
        
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    public bool Remove(Expression<Func<T, bool>> predicate);
}