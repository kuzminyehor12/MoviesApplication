using Movies.Core.Enums;
using Movies.Search.Utils.Generators;

namespace MoviesApplication.Search.Tests;

public class TfIdfTokenGeneratorTests
{
     private readonly TfIdfTokenGenerator _generator = new(FieldType.Overview);

    [Theory]
    [InlineData("The runners are running fast.", new string[] { "runner", "run", "fast" })]
    [InlineData("  word1   word2 ", new string[] { "word1", "word2" })]
    [InlineData("organizes organizing organizational", new string[] { "organ", "organ", "organiz" })]
    [InlineData("A single word", new string[] { "singl", "word" })]
    [InlineData("", new string[] { })]
    [InlineData("  ", new string[] { })]
    public void Generate_VariousInputs_ReturnsCorrectlyStemmedAndNormalizedTokens(string inputText, string[] expectedTokens)
    {
        // Act
        var result = _generator.Generate(inputText);
        var actualTokens = result.Select(t => t.Term).ToArray();
        
        // Assert
        Assert.Equal(expectedTokens.Length, result.Count);
        Assert.Equal(expectedTokens, actualTokens);
        Assert.All(result, token => Assert.Equal(TermType.WholeString, token.TermType));
    }
}