namespace Films.Core.Domain.Filters;

public sealed class MovieFilter : BaseFilter
{
    public bool? Disabled { get; set; }
}