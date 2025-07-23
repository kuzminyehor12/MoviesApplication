using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Movies.Core.Entities;

namespace Movies.IndexBuilder;

class Program
{
    static async Task Main(string[] args)
    {
        // TODO: Create an utility which takes a data from directory and puts it into an inverted index
        // Index should represent this structure:
        // -----------------------------------------------------------
        // | keyword_id | keyword_word | inversed_document_frequency |
        // -----------------------------------------------------------
        // ------------------------------------------
        // | keyword_id | movie_id | term_frequency |
        // ------------------------------------------
        
        // TODO: Use preprocessing for data:
        // - normalize data in ~/Data folder(using jupyter notebook maybe)
        // - do lemmatizing/stemming
        // - remove stop words
        
        // TODO: Keep infrastructure up-to-date(do it after infrastructure will be defined) 
        // - Run container instance of database
        // - During reindexing we have to consider having two instances(one to be available and other to be re-indexed based on updated data)
        // - Consider infrastructure clean up afterwards
        // - Roll back to available instance if indexing error happened
        
        // Create PoC for two cases: MongoDb and Postgres
        // Use benchmarks to determine which index is faster
        
        // Index original title to find exact match
        // If exact match not found try fuzzy search
        // use poster_path with https://image.tmdb.org/t/p/original/
        // use profile_path with https://image.tmdb.org/t/p/w200/
        
        // How to compute score:
        // * bonus for exact match
        // * fuzzy search
        // * bonus for position
        // * scoring threshold
        // * set weights per movie property
        // * TF-IDF vs N-gram?
        
        // Algorithm
        // 1. Add a document(movie)
        // 2. Extract terms/n-grams considering type from the document. IMPORTANT! Separate n-grams from terms for TF-IDF vectorization 
        // 3. Normalize them 
        // 4. Put into terms table
        // 5. Add a relationship between document and a term with a frequency and a position
        // 6. After all document added compute TF-IDF and place it into document_vectors table
        
        Console.WriteLine("Indexing has been started!");

        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };
        
        var moviesData = Directory.EnumerateFiles("output", "*.csv");
        var indexingTasks = new List<Task>();

        int totalCount = 0;
        
        foreach (var file in moviesData)
        {
            var chunkIndexingTask = Task.Run(async () =>
            {
                using var streamReader = File.OpenText(file);
                using var csvReader = new CsvReader(streamReader, csvConfiguration);
                csvReader.Context.RegisterClassMap<CsvMovieMap>();
                
                await foreach (var movie in csvReader.GetRecordsAsync<Movie>())
                {
                    Interlocked.Increment(ref totalCount);
                    Console.WriteLine($"Processing {movie.Title} | {movie.ReleaseDate?.ToShortDateString() ?? "[NO DATE]"}");
                }
            });
            
            indexingTasks.Add(chunkIndexingTask);
        }
        
        await Task.WhenAll(indexingTasks);
        
        Console.WriteLine($"Indexing has been completed! {totalCount} documents was indexed.");
    }
}