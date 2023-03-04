namespace Films.Core.Domain.Pagination;

public class PaginationInfo
{
    public int PageNo { get; }
    public int PageSize { get; }
    public int PageCount { get; }
    public long TotalRecordCount { get; }

    public PaginationInfo(int pageNo, int pageSize, int pageCount, long totalRecordCount)
    {
        PageNo = pageNo;
        PageSize = pageSize;
        PageCount = pageCount;
        TotalRecordCount = totalRecordCount;
    }
}