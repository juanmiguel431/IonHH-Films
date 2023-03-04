using Films.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Films.Persistence.EntitiesConfigurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable(TableName.Review, TableSchemaName.Mov);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.MovieId)
            .IsRequired();
        
        builder.HasOne(p => p.Movie)
            .WithMany(p => p.Reviews)
            .HasForeignKey(p => p.MovieId);

        builder.Property(p => p.Title)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.CreatedDate)
            .IsRequired();
    }
}