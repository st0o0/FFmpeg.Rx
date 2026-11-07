using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Metadata;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class MetadataTests
{
    private static string BuildWith(Action<Builder.OutputOptions> configure)
    {
        var options = new Builder.OutputOptions();
        configure(options);
        var pipeline = new FfmpegPipeline(
            [new FileInput("input.mp4")],
            new FfmpegOutput("out.mp4", [.. options.Mappings], [.. options.Arguments], options.SuppressOverwrite),
            []);
        return ArgumentBuilder.Build(pipeline);
    }

    [Fact]
    public void Stream_metadata()
    {
        var args = BuildWith(o => o.StreamMetadata("title", "English", StreamType.Audio, 0));
        Assert.Contains("-metadata:s:a:0 title=English", args);
    }

    [Fact]
    public void Global_metadata()
    {
        var args = BuildWith(o => o.GlobalMetadata("title", "My Video"));
        Assert.Contains("-metadata title=My Video", args);
    }

    [Fact]
    public void Without_metadata()
    {
        var args = BuildWith(o => o.WithoutMetadata());
        Assert.Contains("-map_metadata -1", args);
    }

    [Fact]
    public void Copy_metadata_from()
    {
        var args = BuildWith(o => o.CopyMetadataFrom(0));
        Assert.Contains("-map_metadata 0", args);
    }

    [Fact]
    public void Disposition_default_audio()
    {
        var args = BuildWith(o => o.Disposition(StreamType.Audio, 0, "default"));
        Assert.Contains("-disposition:a:0 default", args);
    }

    [Fact]
    public void Disposition_forced_subtitle()
    {
        var args = BuildWith(o => o.Disposition(StreamType.Subtitle, 0, "forced"));
        Assert.Contains("-disposition:s:0 forced", args);
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
