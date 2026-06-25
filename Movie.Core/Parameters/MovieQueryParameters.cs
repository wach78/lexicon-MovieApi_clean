namespace Movie.Core.Parameters;

public sealed class MovieQueryParameters : PaginationParameters
{
    public string? Genre { get; set; }

    public int? Year { get; set; }

    public string? Actor { get; set; }
}
