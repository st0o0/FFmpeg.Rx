using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Metadata;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class MetadataTests
{
    private static string SerializeWithMetadata(MetadataConfig metadata)
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4")],
            [new OutputConfig(new FileDestination("out.mp4"), Metadata: metadata)]);
        return ArgumentSerializer.Serialize(spec);
    }

    [Fact]
    public void Global_metadata()
    {
        var args = SerializeWithMetadata(new MetadataConfig(
            GlobalEntries: [new("title", "My Video")]));
        Assert.Contains("-metadata title=\"My Video\"", args);
    }

    [Fact]
    public void Without_metadata()
    {
        var args = SerializeWithMetadata(new MetadataConfig(StripAll: true));
        Assert.Contains("-map_metadata -1", args);
    }

    [Fact]
    public void Copy_metadata_from()
    {
        var args = SerializeWithMetadata(new MetadataConfig(CopyFromInput: 0));
        Assert.Contains("-map_metadata 0", args);
    }

    [Fact]
    public void Stream_metadata_on_stream_spec()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4")],
            [new OutputConfig(new FileDestination("out.mp4"),
                Streams: [new StreamConfig(0, 0, StreamType.Audio, new StreamCopy(),
                    Metadata: [new("title", "English")])])]);
        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map 0:a:0", args);
    }

    [Fact]
    public void FFMetadata_builds_chapters()
    {
        var metadata = new FFMetadataBuilder()
            .WithTitle("Test Video")
            .WithChapter("Intro", TimeSpan.Zero, TimeSpan.FromSeconds(30))
            .WithChapter("Main", TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5))
            .Build();

        Assert.Contains(";FFMETADATA1", metadata);
        Assert.Contains("title=Test Video", metadata);
        Assert.Contains("[CHAPTER]", metadata);
        Assert.Contains("START=0", metadata);
        Assert.Contains("END=30000", metadata);
        Assert.Contains("title=Intro", metadata);
        Assert.Contains("START=30000", metadata);
        Assert.Contains("END=300000", metadata);
        Assert.Contains("title=Main", metadata);
    }

    [Fact]
    public void FFMetadata_sequential_chapters()
    {
        var metadata = new FFMetadataBuilder()
            .WithChapter("Part 1", TimeSpan.FromSeconds(10))
            .WithChapter("Part 2", TimeSpan.FromSeconds(20))
            .Build();

        Assert.Contains("START=0", metadata);
        Assert.Contains("END=10000", metadata);
        Assert.Contains("START=10000", metadata);
        Assert.Contains("END=30000", metadata);
    }
}
