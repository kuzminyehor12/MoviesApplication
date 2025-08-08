namespace Movies.Search.Utils.Generators;

internal interface ITokenGenerator
{
    TokenCollection Generate(string text);
}