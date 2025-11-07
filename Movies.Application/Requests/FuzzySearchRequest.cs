namespace Movies.Application.Requests;

public class FuzzySearchRequest
{
    public required string Query { get; set; }
    
    public required int PageNumber { get; set; }
}