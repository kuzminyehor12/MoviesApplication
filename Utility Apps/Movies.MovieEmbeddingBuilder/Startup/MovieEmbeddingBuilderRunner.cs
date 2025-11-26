using System.Collections.Concurrent;
using Movies.Core.Entities;
using Movies.Persistence.Infrastructure;
using Movies.Search;

namespace Movies.MovieEmbeddingBuilder.Startup;

public class MovieEmbeddingBuilderRunner(IDatabase<MovieDbContext> database, IVectorGenerator vectorGenerator)
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var movies = await database.Store<Movie>().ListAsync(cancellationToken: cancellationToken);
            var embeddings = new List<MovieEmbedding>();

            foreach (var movie in movies)
            {
                var vector = await vectorGenerator.VectorizeAsync(movie.ToString(), cancellationToken);

                var embedding = new MovieEmbedding
                {
                    MovieId = movie.Id,
                    Vector = vector
                };

                embeddings.Add(embedding);
                Console.WriteLine($"Embedding generated for {movie.Id} | {movie.Title}");
            }
            
            await database.Store<MovieEmbedding>().BulkAddAsync(embeddings, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(-1);
        }
    }
}