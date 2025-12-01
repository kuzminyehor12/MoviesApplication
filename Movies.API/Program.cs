using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Movies.Application.Abstractions;
using Movies.Application.Services;
using Movies.Configuration;
using Movies.Configuration.Configurations;
using Movies.Configuration.Database;
using Movies.Migrations;
using Movies.Persistence.Infrastructure;
using Movies.Search;
using Movies.Search.Utils.Normalizers;
using OllamaSharp;

namespace Movies.API;

public static class Program
{
    private static IConfiguration Configuration => MovieConfigurationManager.Configuration;
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddTransient<ITextNormalizer, DefaultNormalizer>();

        builder.Services.Configure<OllamaApiClientSettings>(Configuration.GetSection("OllamaApiClientSettings"));
        
        builder.Services.AddScoped<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var ollamaSettings = sp.GetRequiredService<IOptions<OllamaApiClientSettings>>().Value;
            return new OllamaApiClient(new Uri(ollamaSettings.Url), ollamaSettings.Model);
        });
        
        builder.Services.AddScoped<IFuzzySearchService, FuzzySearchService>();
        builder.Services.AddScoped<ILexicalSearchService, LexicalSearchService>();
        builder.Services.AddScoped<ISemanticSearchService, SemanticSearchService>();
        builder.Services.AddScoped<ISuggestionService, SuggestionsService>();
        builder.Services.AddScoped<IRecommendationService, RecommendationService>();
        builder.Services.AddScoped<ITokenExtractor, TokenExtractor>();
        builder.Services.AddScoped<IVectorGenerator, VectorGenerator>();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "FrontendPolicy",
                policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddConfigurations();
        builder.Services.AddDatabase();
        
        // Add services to the container.
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseCors("FrontendPolicy");

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
    
    private static void AddDatabase(this IServiceCollection services)
    {
        services.AddScoped<IEnvironmentConnectionStringFactory, EnvironmentConnectionStringFactory>();
        
        services.AddScoped<MovieDbContext>(sp =>
        {
            var connectionStringFactory = sp.GetRequiredService<IEnvironmentConnectionStringFactory>();
            return GetMovieDbContext(connectionStringFactory.GetActiveDatabaseEnvironment());
        });
        
        services.AddScoped(typeof(IDataStoreFactory<>), typeof(DataStoreFactory<>));
        
        services.AddScoped(typeof(IDatabase<>), typeof(Database<>));
    }

    private static void AddConfigurations(this IServiceCollection services)
    {
        services.AddSingleton(Configuration);
    }
    
    private static MovieDbContext GetMovieDbContext(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException($"Connection string '{connectionString}' not found.");
        }
        
        var options = MovieDbContextOptionsFactory.CreateDbContextOptions(connectionString);
        return new MovieDbContext(options);
    }
}