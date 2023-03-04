namespace Films.Core.Domain.Dtos;

public class ReviewDto
{
    public long Id { get; set; }
    public long MovieId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Rating Rating { get; set; }
}