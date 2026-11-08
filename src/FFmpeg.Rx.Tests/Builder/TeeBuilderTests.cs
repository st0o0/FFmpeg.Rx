using FFmpeg.Rx.Builder;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class TeeBuilderTests
{
    [Fact]
    public void Single_target_no_options()
    {
        var builder = new TeeBuilder();
        builder.Target("out.mp4");
        var tee = builder.Build();
        Assert.Single(tee.Targets);
        Assert.Equal("out.mp4", tee.Targets[0].Path);
    }

    [Fact]
    public void Two_targets_with_format()
    {
        var builder = new TeeBuilder();
        builder
            .Target("out1.mp4", o => o.ForceFormat("mp4"))
            .Target("out2.mkv", o => o.ForceFormat("matroska"));
        var tee = builder.Build();
        Assert.Equal(2, tee.Targets.Length);
        Assert.Equal("mp4", tee.Targets[0].Format);
        Assert.Equal("matroska", tee.Targets[1].Format);
    }

    [Fact]
    public void Target_with_all_options()
    {
        var builder = new TeeBuilder();
        builder.Target("out.mp4", o => o
            .ForceFormat("mp4")
            .WithSelect("v")
            .WithBitstreamFilter("aac_adtstoasc")
            .WithMovFlags("faststart")
            .OnFailIgnore());
        var tee = builder.Build();
        var target = Assert.Single(tee.Targets);
        Assert.Equal("mp4", target.Format);
        Assert.Equal("v", target.Select);
        Assert.Equal("aac_adtstoasc", target.BitstreamFilter);
        Assert.Equal("faststart", target.MovFlags);
        Assert.True(target.IgnoreFailure);
    }

    [Fact]
    public void Empty_builder_produces_empty_targets()
    {
        var builder = new TeeBuilder();
        var tee = builder.Build();
        Assert.Empty(tee.Targets);
    }
}
