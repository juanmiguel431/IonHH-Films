using Bogus;
using Bogus.Hollywood;
using Films.Core.Domain;
using Films.Persistence.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Films.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoviesAndReviews : Migration
    {
        private const int _moviesCount = 1000;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var moviesTable = $"{TableSchemaName.Mov}.{TableName.Movie}";
            var reviewsTable = $"{TableSchemaName.Mov}.{TableName.Review}";
            migrationBuilder.Sql($"DELETE FROM {moviesTable};");
            migrationBuilder.Sql($"DELETE FROM {reviewsTable};");

            Randomizer.Seed = new Random(8675309);
            
            var reviewIds = 1;
            var reviews = new Faker<Review>()
                .RuleFor(u => u.Id, f => reviewIds++)
                .RuleFor(o => o.Title, f => f.Lorem.Sentence(5))
                .RuleFor(o => o.Description, f => f.Lorem.Paragraph())
                .RuleFor(o => o.Rating, f => f.PickRandom<Rating>())
                .RuleFor(o => o.CreatedDate, f => f.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now));

            var movieIds = 1;
            var movies = new Faker<Movie>()
                .RuleFor(u => u.Id, f => movieIds++)
                .RuleFor(u => u.Name, f => f.Movies().MovieTitle())
                .RuleFor(u => u.Description, f => f.Movies().MovieOverview())
                .RuleFor(u => u.Disabled, f => f.Random.Bool())
                .RuleFor(u => u.ReleaseDate, f => f.Movies().MovieReleaseDate())
                .RuleFor(u => u.CreatedDate, f => f.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now));
            
            var movieList = movies.Generate(_moviesCount);

            migrationBuilder.Sql($"set identity_insert {moviesTable} ON;");
            migrationBuilder.Sql($"DBCC CHECKIDENT ('{moviesTable}', RESEED, 0);");

            foreach (var m in movieList)
            {
                var disabled = m.Disabled ? 1 : 0;
                var script = $"INSERT INTO {moviesTable} ({nameof(Movie.Id)}, {nameof(Movie.Name)}, {nameof(Movie.Description)}, {nameof(Movie.ReleaseDate)}, {nameof(Movie.Disabled)}, {nameof(Movie.CreatedDate)}) values ({m.Id}, '{GetScappedStringValue(m.Name)}', '{GetScappedStringValue(m.Description)}', '{m.ReleaseDate}', {disabled}, '{m.CreatedDate}');";
                migrationBuilder.Sql(script);
            }
            migrationBuilder.Sql($"set identity_insert {moviesTable} OFF;");
            migrationBuilder.Sql($"set identity_insert {reviewsTable} ON;");
            migrationBuilder.Sql($"DBCC CHECKIDENT ('{reviewsTable}', RESEED, 0);");

            foreach (var m in movieList)
            {
                var movieReviews = reviews.Generate(Randomizer.Seed.Next(1, 10));
                foreach (var r in movieReviews)
                {
                    var rating = (int)r.Rating;
                    var script = $"INSERT INTO {reviewsTable} ({nameof(Review.Id)}, {nameof(Review.Title)}, {nameof(Review.Description)}, {nameof(Review.MovieId)}, {nameof(Review.Rating)}, {nameof(Review.CreatedDate)}) values ({r.Id},'{GetScappedStringValue(r.Title)}', '{GetScappedStringValue(r.Description)}', {m.Id}, {rating}, '{r.CreatedDate}');";
                    migrationBuilder.Sql(script);
                }
            }
            
            migrationBuilder.Sql($"set identity_insert {reviewsTable} OFF;");
        }

        private static string GetScappedStringValue(string value)
        {
            return value.Replace("'", "''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var moviesTable = $"{TableSchemaName.Mov}.{TableName.Movie}";
            var reviewsTable = $"{TableSchemaName.Mov}.{TableName.Review}";

            for (int id = 1; id <= _moviesCount; id++)
            {
                migrationBuilder.Sql($"DELETE FROM {moviesTable} WHERE {nameof(Movie.Id)} = {id};");                
            }

            migrationBuilder.Sql($"DBCC CHECKIDENT ('{moviesTable}', RESEED, 0);");
            migrationBuilder.Sql($"DBCC CHECKIDENT ('{reviewsTable}', RESEED, 0);");
        }
    }
}
