using FFmpeg.Rx.Internal;

namespace FFmpeg.Rx.Tests;

public sealed class ErrorClassifierTests
{
    [Theory]
    [InlineData("HTTP error 503 Service Unavailable")]
    [InlineData("HTTP error 502 Bad Gateway")]
    [InlineData("Connection timed out")]
    [InlineData("Connection reset by peer")]
    [InlineData("Connection refused")]
    [InlineData("Network is unreachable")]
    [InlineData("Temporary failure in name resolution")]
    public void Transient_errors_are_classified_correctly(string error) => Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify(error));

    [Theory]
    [InlineData("HTTP error 403 Forbidden")]
    [InlineData("Invalid data found when processing input")]
    [InlineData("No such file or directory")]
    [InlineData("Something completely unknown")]
    public void Permanent_errors_are_classified_correctly(string error) => Assert.Equal(FailureKind.Permanent, ErrorClassifier.Classify(error));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Null_or_empty_is_permanent(string? error) => Assert.Equal(FailureKind.Permanent, ErrorClassifier.Classify(error));

    [Fact]
    public void ExtractError_finds_http_error_line()
    {
        var stderr = "frame=0\nHTTP error 403 Forbidden\nfoo";
        Assert.Equal("HTTP error 403 Forbidden", ErrorClassifier.ExtractError(stderr));
    }

    [Fact]
    public void ExtractError_returns_last_line_when_no_pattern()
    {
        var stderr = "line1\nline2\nline3";
        Assert.Equal("line3", ErrorClassifier.ExtractError(stderr));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ExtractError_returns_empty_for_null_or_whitespace(string? stderr) => Assert.Equal("", ErrorClassifier.ExtractError(stderr));
}
