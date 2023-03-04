namespace Films.Core.Domain.Pagination;

public class PaginatedList<T>
{
    public IReadOnlyList<T> Data { get; }
    public PaginationInfo Paging { get; }

    public PaginatedList(IReadOnlyList<T> data, PaginationInfo paging)
    {
        Data = data;
        Paging = paging;
    }
}