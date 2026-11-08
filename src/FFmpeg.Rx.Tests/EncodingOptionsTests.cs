using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class EncodingOptionsTests
{
    private static string SerializeWith(Action<OutputConfig> _setup, OutputConfig output)
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4")],
            [output]);
        return ArgumentSerializer.Serialize(spec);
    }

    private static string SerializeOutput(OutputConfig output)
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4")],
            [output]);
        return ArgumentSerializer.Serialize(spec);
    }

    [Fact]
    public void Crf_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Quality: Quality.Crf(23))))]));
        Assert.Contains("-crf 23", args);
    }

    [Fact]
    public void Video_bitrate_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Quality: Quality.Bitrate(Bitrate.Mbps(4)))))]));
        Assert.Contains("-b:v 4M", args);
    }

    [Fact]
    public void Audio_bitrate_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Audio,
                new AudioEncode(AudioCodec.Aac, new AudioEncodingOptions(Bitrate: Bitrate.Kbps(192))))]));
        Assert.Contains("-b:a 192k", args);
    }

    [Fact]
    public void Framerate_int_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Framerate: Framerate.Fps(30))))]));
        Assert.Contains("-r 30", args);
    }

    [Fact]
    public void Framerate_rational_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Framerate: Framerate.Rational(30000, 1001))))]));
        Assert.Contains("-r 30000/1001", args);
    }

    [Fact]
    public void Preset_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Preset: Preset.Slow)))]));
        Assert.Contains("-preset slow", args);
    }

    [Fact]
    public void Profile_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Profile: VideoProfile.High)))]));
        Assert.Contains("-profile:v high", args);
    }

    [Fact]
    public void Pixel_format_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(PixelFormat: PixelFormat.Yuv420p)))]));
        Assert.Contains("-pix_fmt yuv420p", args);
    }

    [Fact]
    public void Container_format_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.ts"),
            Container: ContainerFormat.Ts));
        Assert.Contains("-f mpegts", args);
    }

    [Fact]
    public void Tune_emits_flag()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [new StreamConfig(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Tune: Tune.Film)))]));
        Assert.Contains("-tune film", args);
    }

    [Fact]
    public void Default_overwrite()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4")));
        Assert.Contains("-y \"out.mp4\"", args);
    }

    [Fact]
    public void No_overwrite()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"), Overwrite: false));
        Assert.Contains("-n \"out.mp4\"", args);
        Assert.DoesNotContain("-y", args);
    }

    [Fact]
    public void Video_codec_struct_names()
    {
        Assert.Equal("libx264", VideoCodec.H264.Name);
        Assert.Equal("libx265", VideoCodec.Hevc.Name);
        Assert.Equal("libvpx-vp9", VideoCodec.Vp9.Name);
        Assert.Equal("libaom-av1", VideoCodec.Av1.Name);
        Assert.Equal("copy", VideoCodec.Copy.Name);
        Assert.Equal("h264_nvenc", VideoCodec.H264Nvenc.Name);
    }

    [Fact]
    public void Audio_codec_struct_names()
    {
        Assert.Equal("aac", AudioCodec.Aac.Name);
        Assert.Equal("libopus", AudioCodec.Opus.Name);
        Assert.Equal("libmp3lame", AudioCodec.Mp3.Name);
        Assert.Equal("flac", AudioCodec.Flac.Name);
        Assert.Equal("copy", AudioCodec.Copy.Name);
    }

    [Fact]
    public void Subtitle_codec_struct_names()
    {
        Assert.Equal("mov_text", SubtitleCodec.MovText.Name);
        Assert.Equal("srt", SubtitleCodec.Srt.Name);
        Assert.Equal("ass", SubtitleCodec.Ass.Name);
        Assert.Equal("webvtt", SubtitleCodec.WebVtt.Name);
        Assert.Equal("copy", SubtitleCodec.Copy.Name);
    }

    [Fact]
    public void Full_encoding_pipeline()
    {
        var args = SerializeOutput(new OutputConfig(new FileDestination("out.mp4"),
            Streams: [
                new StreamConfig(0, 0, StreamType.Video,
                    new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                        Quality: Quality.Crf(23),
                        Preset: Preset.Medium,
                        Profile: VideoProfile.High,
                        PixelFormat: PixelFormat.Yuv420p,
                        Tune: Tune.Film))),
                new StreamConfig(0, 0, StreamType.Audio,
                    new AudioEncode(AudioCodec.Aac, new AudioEncodingOptions(Bitrate: Bitrate.Kbps(192))))]));

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
