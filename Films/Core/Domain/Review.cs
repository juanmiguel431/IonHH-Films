namespace Films.Core.Domain;

public class Review
{
    public long Id { get; set; }
    public long MovieId { get; set; }
    public Movie Movie { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
}