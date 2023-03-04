using Films.Core.Domain;
using Films.Core.Domain.Filters;

namespace Films.Core.Repositories;

public interface IReviewRepository : IRepository<Review, ReviewFilter>
{
}