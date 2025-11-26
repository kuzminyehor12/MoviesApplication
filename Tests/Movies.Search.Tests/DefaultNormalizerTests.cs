using Movies.Search.Utils.Normalizers;

namespace MoviesApplication.Search.Tests;

public class DefaultNormalizerTests
{
    private readonly DefaultNormalizer _normalizer = new();

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(null)]
    public void Normalize_ShouldHandleNullOrEmptyInput(string? input)
    {
        // Act & Assert
        Assert.Equal(string.Empty, _normalizer.Normalize(input));
    }

    [Theory]
    [InlineData("HELLO WORLD", "hello world")]
    [InlineData("  leading and trailing  ", "leading and trailing")]
    [InlineData("MiXeD cAsE", "mixed case")]
    public void Normalize_ShouldHandleCaseAndTrimming(string input, string expected)
    {
        // Act
        string result = _normalizer.Normalize(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("été", "ete")]
    [InlineData("über", "uber")]
    [InlineData("Español", "espanol")]
    [InlineData("façade", "facade")]
    [InlineData("Crème Brûlée", "creme brulee")]
    public void Normalize_ShouldRemoveDiacritics(string input, string expected)
    {
        // Act
        string result = _normalizer.Normalize(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Don't Stop Believing", "dont stop believing")]
    [InlineData("I'm so excited!", "im so excited")]
    [InlineData("L’amour", "lamour")] // Test curly apostrophe
    public void Normalize_ShouldNormalizeApostrophes(string input, string expected)
    {
        // Act
        string result = _normalizer.Normalize(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello, World!", "hello world")]
    [InlineData("It's a test-case.", "its a test case")]
    [InlineData("100% complete!", "100 complete")]
    [InlineData("The title: A New Hope", "the title a new hope")]
    public void Normalize_ShouldRemovePunctuation(string input, string expected)
    {
        // Act
        string result = _normalizer.Normalize(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("The   quick   brown    fox", "the quick brown fox")]
    [InlineData("  The   quick brown fox  ", "the quick brown fox")]
    public void Normalize_ShouldCollapseMultipleSpaces(string input, string expected)
    {
        // Act
        string result = _normalizer.Normalize(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C’est la vie!", "cest la vie")]
    [InlineData("Pôle Position (1983)", "pole position 1983")]
    [InlineData("THE MATRIX RELOADED!! ", "the matrix reloaded")]
    public void Normalize_ShouldHandleCombinedScenarios(string input, string expected)
    {
        // Act
        string result = _normalizer.Normalize(input);

        // Assert
        Assert.Equal(expected, result);
    }
}