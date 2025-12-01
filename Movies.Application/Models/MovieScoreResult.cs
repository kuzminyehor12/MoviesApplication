namespace Movies.Application.Models;

public class MovieScoreResult
{
    private const decimal MaxThreshold = 1m;
    private decimal _score;
    
    public required MovieViewModel Movie { get; set; }

    public required decimal Score
    {
        get =>  _score;
        set => _score = Math.Min(value, MaxThreshold);
    }
}