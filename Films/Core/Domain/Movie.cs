namespace Films.Core.Domain;

public class Movie
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool Disabled { get; set; }
    public List<Review> Reviews { get; set; }

    public Movie()
    {
        Reviews = new List<Review>();
    }
}