namespace Movies.Application.Models;

public class PaginatedResult<T>
{
    public const int DefaultPageSize = 20;
    
    public required IReadOnlyCollection<T>? Results { get; init; }
    
    public int ItemsCount => Results?.Count ?? 0;
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
    
    public int TotalPages { get; init; }

    public static PaginatedResult<T> Create(IEnumerable<T> results, int pageNumber = 1)
    {
        var items = results.ToList();
        
        return new PaginatedResult<T>
        {
            Results = items.Skip((pageNumber - 1) * DefaultPageSize).Take(DefaultPageSize).ToArray(),
            PageNumber = pageNumber,
            PageSize = DefaultPageSize,
            TotalPages = (int)Math.Ceiling(items.Count / (double)DefaultPageSize)
        };
    }
}