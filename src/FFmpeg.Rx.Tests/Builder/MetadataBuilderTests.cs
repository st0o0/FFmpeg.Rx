using FFmpeg.Rx.Builder;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class MetadataBuilderTests
{
    [Fact]
    public void Global_adds_entry()
    {
        var builder = new MetadataBuilder();
        builder.Global("title", "Test");
        var spec = builder.Build();
        Assert.NotNull(spec.GlobalEntries);
        var entry = Assert.Single(spec.GlobalEntries);
        Assert.Equal("title", entry.Key);
        Assert.Equal("Test", entry.Value);
    }

    [Fact]
    public void Global_multiple_entries()
    {
        var builder = new MetadataBuilder();
        builder.Global("title", "Test").Global("artist", "Me");
        var spec = builder.Build();
        Assert.NotNull(spec.GlobalEntries);
        Assert.Equal(2, spec.GlobalEntries.Count);
    }

    [Fact]
    public void StripAll_sets_flag()
    {
        var builder = new MetadataBuilder();
        builder.StripAll();
        var spec = builder.Build();
        Assert.True(spec.StripAll);
    }

    [Fact]
    public void CopyFrom_sets_input()
    {
        var builder = new MetadataBuilder();
        builder.CopyFrom(0);
        var spec = builder.Build();
        Assert.Equal(0, spec.CopyFromInput);
    }

    [Fact]
    public void Defaults()
    {
        var builder = new MetadataBuilder();
        var spec = builder.Build();
        Assert.Null(spec.GlobalEntries);
        Assert.False(spec.StripAll);
        Assert.Null(spec.CopyFromInput);
    }
}
