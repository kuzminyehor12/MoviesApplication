using Movies.Search.Utils.Generators;

namespace MoviesApplication.Search.Tests;

public class NgramGeneratorTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData(null)]
    public void GenerateByCharacters_ShouldReturnEmpty_ForNullOrEmptyInput(string? input)
    {
        // Act
        string[] result = NgramGenerator.GenerateByCharacters(input);

        // Assert
        Assert.Empty(result);
    }
    

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void GenerateByCharacters_ShouldReturnEmpty_ForTermShorterThanNgramLength(string term)
    {
        // Act
        string[] result = NgramGenerator.GenerateByCharacters(term);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("abc", new[] { "abc" })]
    [InlineData("abcd", new[] { "abc", "bcd" })]
    [InlineData("apple", new[] { "app", "ppl", "ple" })]
    [InlineData("star wars", new[] { "sta", "tar", "ar ", "r w", " wa", "war", "ars" })]
    public void GenerateByCharacters_ShouldReturnCorrectNgrams(string term, string[] expectedNgrams)
    {
        // Act
        string[] result = NgramGenerator.GenerateByCharacters(term);

        // Assert
        Assert.Equal(expectedNgrams, result);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData(null)]
    public void GenerateByWords_ShouldReturnEmpty_ForNullOrEmptyInput(string? input)
    {
        // Act
        string[] result = NgramGenerator.GenerateByWords(input);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("one")]
    [InlineData("   ")]
    public void GenerateByWords_ShouldReturnEmpty_ForTermShorterThanNgramLength(string term)
    {
        // Act
        string[] result = NgramGenerator.GenerateByWords(term);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("two words", new[] { "two words" })]
    [InlineData("the quick brown fox", new[] { "the quick", "quick brown", "brown fox" })]
    [InlineData("  a   lot   of spaces  ", new[] { "a lot", "lot of", "of spaces" })]
    public void GenerateByWords_ShouldReturnCorrectNgrams(string term, string[] expectedNgrams)
    {
        // Act
        string[] result = NgramGenerator.GenerateByWords(term);

        // Assert
        Assert.Equal(expectedNgrams, result);
    }
}