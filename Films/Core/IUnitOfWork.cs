using Films.Core.Repositories;

namespace Films.Core;

public interface IUnitOfWork : IDisposable
{
    IMovieRepository Movies { get; }
    IReviewRepository Reviews { get; }
    int SaveChanges();
}