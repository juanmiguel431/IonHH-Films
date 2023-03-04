namespace Films.Core.Domain.Filters;

public class BaseFilter
{
    public long? Id { get; set; }
    public string Search { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public string SortField { get; set; }
    public string SortDir { get; set; }
}