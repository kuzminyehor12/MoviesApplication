using Microsoft.Extensions.Configuration;

namespace Movies.Configuration.Database;

public class EnvironmentConnectionStringFactory(IConfiguration configuration) : IEnvironmentConnectionStringFactory
{
    private const string GreenEnvironment = "Green";
    private const string BlueEnvironment = "Blue";
    
    private const string DbConnectionSettingNameSuffix = "MovieDbConnection";
    private const string ActiveDatabaseEnvironmentSettingName = "ActiveDatabaseEnvironment";
    
    public string GetActiveDatabaseEnvironment()
    {
        string activeEnv = GetActiveEnvironmentConfiguration();
        return configuration.GetConnectionString($"{activeEnv}{DbConnectionSettingNameSuffix}") ??  throw new InvalidOperationException($"{activeEnv}{DbConnectionSettingNameSuffix} not found.");
    }

    public string GetInactiveDatabaseEnvironment()
    {
        string activeEnv = GetActiveEnvironmentConfiguration();
        string inactiveEnv = activeEnv == BlueEnvironment ? GreenEnvironment : BlueEnvironment;
        return configuration.GetConnectionString($"{inactiveEnv}{DbConnectionSettingNameSuffix}") ??  throw new InvalidOperationException($"{inactiveEnv}{DbConnectionSettingNameSuffix} not found.");
    }

    private string GetActiveEnvironmentConfiguration()
    {
        string? activeEnv = configuration[ActiveDatabaseEnvironmentSettingName];

        if (string.IsNullOrEmpty(activeEnv))
        {
            throw new InvalidOperationException($"\"{ActiveDatabaseEnvironmentSettingName}\" not found.");
        }
        
        return activeEnv;
    }
}