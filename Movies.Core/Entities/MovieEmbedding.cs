namespace Movies.Core.Entities;

public class MovieEmbedding : BaseEntity<int>
{
    public int MovieId { get; init; }
    public Movie Movie { get; init; }
    
    public float[] Vector { get; init; }
}