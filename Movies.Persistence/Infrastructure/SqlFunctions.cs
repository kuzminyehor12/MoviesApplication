namespace Movies.Persistence.Infrastructure;

public static class SqlFunctions
{
    public static decimal CalculateJaccard(int movieId, string[] queryNgrams) 
        => throw new NotSupportedException();
    
    public static decimal CalculateDice(int movieId, string[] queryNgrams, double bonus = 0.5) 
        => throw new NotSupportedException();
}