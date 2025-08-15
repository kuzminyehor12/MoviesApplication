namespace Movies.Configuration.Database;

public interface IEnvironmentConnectionStringFactory
{
    string GetActiveDatabaseEnvironment();
    
    string GetInactiveDatabaseEnvironment();
}