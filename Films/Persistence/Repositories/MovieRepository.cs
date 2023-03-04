using Films.Core.Domain;
using Films.Core.Domain.Filters;
using Films.Core.Repositories;

namespace Films.Persistence.Repositories;

public class MovieRepository : Repository<ApplicationDbContext, Movie, BaseFilter> , IMovieRepository
{
    public MovieRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Movie> GetQuery(BaseFilter filter)
    {
        IQueryable<Movie> query = Context.Movies;

        if (filter.Id.HasValue)
        {
            query = query.Where(p => p.Id == filter.Id);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchText = filter.Search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchText));
        }
        
        query = GetQueryOrdered(filter, query);

        return query;
    }

    private static IQueryable<Movie> GetQueryOrdered(BaseFilter filter, IQueryable<Movie> query)
    {
        if (string.IsNullOrEmpty(filter.SortField)) return query;

        query = filter.SortField switch
        {
            nameof(Movie.Id) => filter.SortDir == SortDir.Desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
            nameof(Movie.Name) => filter.SortDir == SortDir.Desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            nameof(Movie.CreatedDate) => filter.SortDir == SortDir.Desc ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
            nameof(Movie.ReleaseDate) => filter.SortDir == SortDir.Desc ? query.OrderByDescending(p => p.ReleaseDate) : query.OrderBy(p => p.ReleaseDate),
            _ => query
        };

        return query;
    }
}