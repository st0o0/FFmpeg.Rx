using FFmpeg.Rx.Filters;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class FilterTests
{
    [Fact]
    public void Scale_renders() => Assert.Equal("scale=1920:1080", new ScaleFilter(1920, 1080).Render());

    [Fact]
    public void Scale_auto_width() => Assert.Equal("scale=1280:-2", new ScaleDimensionFilter(1280, ScaleDimension.Width).Render());

    [Fact]
    public void Scale_auto_height() => Assert.Equal("scale=-2:720", new ScaleDimensionFilter(720, ScaleDimension.Height).Render());

    [Fact]
    public void Crop_renders() => Assert.Equal("crop=640:480:100:50", new CropFilter(640, 480, 100, 50).Render());

    [Fact]
    public void HFlip_renders() => Assert.Equal("hflip", new HFlipFilter().Render());

    [Fact]
    public void VFlip_renders() => Assert.Equal("vflip", new VFlipFilter().Render());

    [Fact]
    public void Transpose_clock() => Assert.Equal("transpose=1", new TransposeFilter(TransposeMode.Clock).Render());

    [Fact]
    public void Fade_in_frames() => Assert.Equal("fade=t=in:s=0:n=30", new FadeFilter(FadeType.In, 0, 30).Render());

    [Fact]
    public void Fade_out_time()
    {
        var f = new FadeFilter(FadeType.Out, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
        Assert.Equal("fade=t=out:st=10.000:d=2.000", f.Render());
    }

    [Fact]
    public void Pad_renders() => Assert.Equal("pad=1920:1080:-1:-1:black", new PadFilter(1920, 1080, -1, -1, "black").Render());

    [Fact]
    public void Yadif_renders() => Assert.Equal("yadif", new YadifFilter().Render());

    [Fact]
    public void Volume_renders() => Assert.Equal("volume=0.8", new VolumeFilter(0.8).Render());

    [Fact]
    public void LoudNorm_default() => Assert.Equal("loudnorm", new LoudNormFilter().Render());

    [Fact]
    public void LoudNorm_custom() => Assert.Equal("loudnorm=I=-16:TP=-1.5:LRA=11", new LoudNormFilter(-16, -1.5, 11).Render());

    [Fact]
    public void AResample_renders() => Assert.Equal("aresample=48000", new AResampleFilter(48000).Render());

    [Fact]
    public void SilenceDetect_renders() => Assert.Equal("silencedetect=n=-30dB:d=2", new SilenceDetectFilter(-30, 2.0).Render());

    [Fact]
    public void Custom_with_args() => Assert.Equal("unsharp=5:5:1.0", new CustomFilter("unsharp", "5:5:1.0").Render());

    [Fact]
    public void Custom_without_args() => Assert.Equal("pp", new CustomFilter("pp").Render());

    [Fact]
    public void Video_filter_chain_serialization()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("in.mp4")],
            [new OutputConfig(
                new FileDestination("out.mp4"),
                Filters: new SimpleFilterConfig(
                    VideoFilters: [new ScaleFilter(1920, 1080), new HFlipFilter()]))]);
        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-vf \"scale=1920:1080,hflip\"", args);
    }

    [Fact]
    public void Audio_filter_chain_serialization()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("in.mp4")],
            [new OutputConfig(
                new FileDestination("out.mp4"),
                Filters: new SimpleFilterConfig(
                    AudioFilters: [new LoudNormFilter(), new VolumeFilter(0.9)]))]);
        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-af \"loudnorm,volume=0.9\"", args);
    }

    [Fact]
    public void Single_video_filter()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("in.mp4")],
            [new OutputConfig(
                new FileDestination("out.mp4"),
                Filters: new SimpleFilterConfig(
                    VideoFilters: [new ScaleFilter(1920, 1080)]))]);
        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-vf \"scale=1920:1080\"", args);
    }

    [Fact]
    public void Label_mapping()
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("in.mp4")],
            [new OutputConfig(
                new FileDestination("out.mp4"),
                LabelMappings: ["out"])]);
        var args = ArgumentSerializer.Serialize(spec);
        Assert.Contains("-map \"[out]\"", args);
    }
}
