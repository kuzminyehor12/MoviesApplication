namespace Movies.Application.Models;

public class Suggestion
{
    public int MovieId { get; set; }
    
    public required string Title { get; set; }
    
    public float VoteAverage { get; set; }
}