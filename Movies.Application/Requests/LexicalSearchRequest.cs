namespace Movies.Application.Requests;

public class LexicalSearchRequest
{
    public required string Query { get; set; }

    public required int PageNumber { get; set; } = 1;
}