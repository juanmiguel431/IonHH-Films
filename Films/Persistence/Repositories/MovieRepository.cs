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
        
        query = GetQueryOrdered(query, filter.SortField, filter.SortDir);

        return query;
    }

    private static IQueryable<Movie> GetQueryOrdered(IQueryable<Movie> query, string? sortField, string? sortDir)
    {
        if (string.IsNullOrEmpty(sortField)) return query;

        query = sortField switch
        {
            nameof(Movie.Id) => sortDir == SortDir.Desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
            nameof(Movie.Name) => sortDir == SortDir.Desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            nameof(Movie.CreatedDate) => sortDir == SortDir.Desc ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
            nameof(Movie.ReleaseDate) => sortDir == SortDir.Desc ? query.OrderByDescending(p => p.ReleaseDate) : query.OrderBy(p => p.ReleaseDate),
            _ => query
        };

        return query;
    }
}