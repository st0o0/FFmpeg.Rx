using FFmpeg.Rx.Internal;

namespace FFmpeg.Rx.Tests;

public sealed class ProgressParserTests
{
    [Fact]
    public void Complete_block_produces_progress_event()
    {
        var block = new Dictionary<string, string>();
        Assert.Null(ProgressParser.ParseLine("total_size=5000000", block));
        Assert.Null(ProgressParser.ParseLine("out_time_us=3000000", block));
        Assert.Null(ProgressParser.ParseLine("speed=1.5x", block));
        var result = ProgressParser.ParseLine("progress=continue", block);
        Assert.NotNull(result);
        Assert.Equal(5000000, result.BytesWritten);
        Assert.Equal(3000000, result.TimeUs);
        Assert.Equal(1.5, result.Speed);
    }

    [Fact]
    public void Partial_block_produces_no_event()
    {
        var block = new Dictionary<string, string>();
        Assert.Null(ProgressParser.ParseLine("total_size=5000000", block));
        Assert.Null(ProgressParser.ParseLine("out_time_us=3000000", block));
    }

    [Fact]
    public void Speed_NA_returns_zero()
    {
        var block = new Dictionary<string, string>();
        ProgressParser.ParseLine("total_size=100", block);
        ProgressParser.ParseLine("out_time_us=200", block);
        ProgressParser.ParseLine("speed=N/A", block);
        var result = ProgressParser.ParseLine("progress=continue", block);
        Assert.NotNull(result);
        Assert.Equal(0.0, result.Speed);
    }

    [Fact]
    public void Missing_fields_default_to_zero()
    {
        var block = new Dictionary<string, string>();
        var result = ProgressParser.ParseLine("progress=end", block);
        Assert.NotNull(result);
        Assert.Equal(0, result.BytesWritten);
        Assert.Equal(0, result.TimeUs);
        Assert.Equal(0.0, result.Speed);
    }

    [Fact]
    public void Multiple_blocks_produce_independent_events()
    {
        var block = new Dictionary<string, string>();
        ProgressParser.ParseLine("total_size=100", block);
        ProgressParser.ParseLine("progress=continue", block);
        ProgressParser.ParseLine("total_size=200", block);
        var result = ProgressParser.ParseLine("progress=continue", block);
        Assert.NotNull(result);
        Assert.Equal(200, result.BytesWritten);
    }

    [Fact]
    public void Malformed_line_without_equals_is_ignored()
    {
        var block = new Dictionary<string, string>();
        Assert.Null(ProgressParser.ParseLine("some warning message", block));
        Assert.Null(ProgressParser.ParseLine("", block));
    }
}
