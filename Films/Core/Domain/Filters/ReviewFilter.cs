namespace Films.Core.Domain.Filters;

public sealed class ReviewFilter : BaseFilter
{
    public long? MovieId { get; set; }
}