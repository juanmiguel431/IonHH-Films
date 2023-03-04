using Films.Core.Domain;
using Films.Persistence.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Films.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
    }
    
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Review> Reviews { get; set; }
}