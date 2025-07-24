namespace Movies.IndexBuilder.Startup;

public class IndexingDataConfiguration
{
    public required string DirectoryPath { get; init; }
    
    public string? SearchPattern { get; init; }
}