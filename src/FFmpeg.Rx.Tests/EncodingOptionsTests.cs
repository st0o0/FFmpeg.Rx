using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class EncodingOptionsTests
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
    public void Crf_emits_flag()
    {
        var args = BuildWith(o => o.Crf(23));
        Assert.Contains("-crf 23", args);
    }

    [Fact]
    public void Video_bitrate_emits_flag()
    {
        var args = BuildWith(o => o.VideoBitrate("4M"));
        Assert.Contains("-b:v 4M", args);
    }

    [Fact]
    public void Audio_bitrate_emits_flag()
    {
        var args = BuildWith(o => o.AudioBitrate("192k"));
        Assert.Contains("-b:a 192k", args);
    }

    [Fact]
    public void Framerate_double_emits_flag()
    {
        var args = BuildWith(o => o.Framerate(30));
        Assert.Contains("-r 30", args);
    }

    [Fact]
    public void Framerate_rational_emits_flag()
    {
        var args = BuildWith(o => o.Framerate("30000/1001"));
        Assert.Contains("-r 30000/1001", args);
    }

    [Fact]
    public void Preset_emits_flag()
    {
        var args = BuildWith(o => o.Preset(Preset.Slow));
        Assert.Contains("-preset slow", args);
    }

    [Fact]
    public void Profile_emits_flag()
    {
        var args = BuildWith(o => o.Profile(VideoProfile.High));
        Assert.Contains("-profile:v high", args);
    }

    [Fact]
    public void Pixel_format_emits_flag()
    {
        var args = BuildWith(o => o.PixelFormat(PixelFormat.Yuv420p));
        Assert.Contains("-pix_fmt yuv420p", args);
    }

    [Fact]
    public void Force_format_emits_flag()
    {
        var args = BuildWith(o => o.ForceFormat("mpegts"));
        Assert.Contains("-f mpegts", args);
    }

    [Fact]
    public void Tune_emits_flag()
    {
        var args = BuildWith(o => o.Tune(EncoderTune.Film));
        Assert.Contains("-tune film", args);
    }

    [Fact]
    public void Default_overwrite()
    {
        var args = BuildWith(o => { });
        Assert.Contains("-y \"out.mp4\"", args);
    }

    [Fact]
    public void No_overwrite()
    {
        var args = BuildWith(o => o.NoOverwrite());
        Assert.Contains("-n \"out.mp4\"", args);
        Assert.DoesNotContain("-y", args);
    }

    [Fact]
    public void Video_codec_constants()
    {
        Assert.Equal("libx264", VideoCodec.LibX264);
        Assert.Equal("libx265", VideoCodec.LibX265);
        Assert.Equal("libvpx-vp9", VideoCodec.LibVpxVp9);
        Assert.Equal("libaom-av1", VideoCodec.LibAomAv1);
        Assert.Equal("copy", VideoCodec.Copy);
        Assert.Equal("h264_nvenc", VideoCodec.H264Nvenc);
    }

    [Fact]
    public void Audio_codec_constants()
    {
        Assert.Equal("aac", AudioCodec.Aac);
        Assert.Equal("libopus", AudioCodec.LibOpus);
        Assert.Equal("libmp3lame", AudioCodec.LibMp3Lame);
        Assert.Equal("flac", AudioCodec.Flac);
        Assert.Equal("copy", AudioCodec.Copy);
    }

    [Fact]
    public void Subtitle_codec_constants()
    {
        Assert.Equal("mov_text", SubtitleCodec.MovText);
        Assert.Equal("srt", SubtitleCodec.Srt);
        Assert.Equal("ass", SubtitleCodec.Ass);
        Assert.Equal("webvtt", SubtitleCodec.WebVtt);
        Assert.Equal("copy", SubtitleCodec.Copy);
    }

    [Fact]
    public void Full_encoding_pipeline()
    {
        var args = BuildWith(o => o
            .Map(0, 0, StreamType.Video, VideoCodec.LibX264)
            .Map(0, 0, StreamType.Audio, AudioCodec.Aac)
            .Crf(23)
            .Preset(Preset.Medium)
            .Profile(VideoProfile.High)
            .PixelFormat(PixelFormat.Yuv420p)
            .AudioBitrate("192k")
            .Tune(EncoderTune.Film));

        Assert.Contains("-c:v libx264", args);
        Assert.Contains("-c:a aac", args);
        Assert.Contains("-crf 23", args);
        Assert.Contains("-preset medium", args);
        Assert.Contains("-profile:v high", args);
        Assert.Contains("-pix_fmt yuv420p", args);
        Assert.Contains("-b:a 192k", args);
        Assert.Contains("-tune film", args);
    }
}
