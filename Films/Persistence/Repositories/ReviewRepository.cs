using Films.Core.Domain;
using Films.Core.Domain.Filters;
using Films.Core.Repositories;

namespace Films.Persistence.Repositories;

public class ReviewRepository : Repository<ApplicationDbContext, Review, ReviewFilter> , IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Review> GetQuery(ReviewFilter filter)
    {
        IQueryable<Review> query = Context.Reviews;

        if (filter.Id.HasValue)
        {
            query = query.Where(p => p.Id == filter.Id);
        }
        
        if (filter.MovieId.HasValue)
        {
            query = query.Where(p => p.MovieId == filter.MovieId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchText = filter.Search.Trim().ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(searchText));
        }
        
        query = GetQueryOrdered(query, filter.SortField, filter.SortDir);

        return query;
    }

    private static IQueryable<Review> GetQueryOrdered(IQueryable<Review> query, string? sortField, string? sortDir)
    {
        if (string.IsNullOrEmpty(sortField)) return query;

        query = sortField switch
        {
            nameof(Movie.Id) => sortDir == SortDir.Desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
            nameof(Movie.Name) => sortDir == SortDir.Desc ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            nameof(Movie.CreatedDate) => sortDir == SortDir.Desc ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
            _ => query
        };

        return query;
    }
}