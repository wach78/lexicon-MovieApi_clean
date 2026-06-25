namespace Movie.Core.Pagination;

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }

    public required int TotalItems { get; init; }

    public required int CurrentPage { get; init; }

    public required int TotalPages { get; init; }

    public required int PageSize { get; init; }
}
