using CsvHelper.Configuration;
using Movies.Core.Entities;
using Movies.IndexBuilder.Converters;

namespace Movies.IndexBuilder;

public sealed class CsvMovieMap : ClassMap<Movie>
{
    public CsvMovieMap()
    {
        Map(m => m.Id).Name("id").TypeConverter<CsvIntConverter>();
        Map(m => m.Title).Name("title");
        Map(m => m.Genres).Name("genres");
        Map(m => m.Overview).Name("overview");
        Map(m => m.Popularity).Name("popularity");
        Map(m => m.ReleaseDate).Name("release_date");
        Map(m => m.TagLine).Name("tagline");
        Map(m => m.CastMembers).Name("cast").TypeConverter<CsvObjectArrayConverter<CastMember[]>>();
        Map(m => m.CrewMembers).Name("crew").TypeConverter<CsvObjectArrayConverter<CrewMember[]>>();
        Map(m => m.Keywords).Name("keywords").TypeConverter<CsvObjectArrayConverter<Keyword[]>>();
        Map(m => m.VoteAverage).Name("vote_average");
        Map(m => m.VoteCount).Name("vote_count").TypeConverter<CsvIntConverter>();
    }
}