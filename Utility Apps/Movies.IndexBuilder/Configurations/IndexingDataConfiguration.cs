namespace Movies.IndexBuilder.Configurations;

public class IndexingDataConfiguration
{
    public required string DirectoryPath { get; init; }
    
    public string? SearchPattern { get; init; }
}