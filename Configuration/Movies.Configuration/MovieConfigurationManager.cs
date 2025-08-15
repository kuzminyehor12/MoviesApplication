using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Movies.Configuration;

public static class MovieConfigurationManager
{
    private static IConfiguration? _configuration;
    private static readonly Lock Locker = new();
    
    public static IConfiguration Configuration
    {
        get
        {
            lock (Locker)
            {
                return _configuration ??= new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.common.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets(typeof(MovieConfigurationManager).Assembly)
                    .Build();
            }
        }
    }
}