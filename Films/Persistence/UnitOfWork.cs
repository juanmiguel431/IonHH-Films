using Films.Core;
using Films.Core.Repositories;
using Films.Persistence.Repositories;

namespace Films.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IMovieRepository? _movies;
    private IReviewRepository? _reviews;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IMovieRepository Movies => _movies ??= new MovieRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
    
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}