using System.ComponentModel.DataAnnotations;
using Movie.Core.Constants;

namespace Movie.Core.Parameters;

public class PaginationParameters
{
    private int _pageSize = PaginationConstants.DefaultPageSize;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = PaginationConstants.DefaultPage;

    [Range(1, int.MaxValue)]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Min(
            value,
            PaginationConstants.MaximumPageSize
        );
    }
}
