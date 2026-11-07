using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Tests;

public sealed class PipelineBuilderTests
{
    [Fact]
    public void Basic_pipeline_builds_session()
    {
        var session = Ffmpeg
            .Input(new Uri("https://example.com/video.mp4"))
            .Output("out.mkv", o => o
                .Map(0, 0, StreamType.Video, VideoCodec.Copy)
                .Map(0, 0, StreamType.Audio, VideoCodec.Copy));

        Assert.NotNull(session);
    }

    [Fact]
    public void Input_options_are_applied()
    {
        var session = Ffmpeg
            .Input(new Uri("https://example.com/v.mp4"), i => i
                .UserAgent("TestAgent")
                .Proxy("http://proxy:8080"))
            .Output("out.mkv", o => o
                .Map(0, 0, StreamType.Video, VideoCodec.Copy));

        Assert.NotNull(session);
    }

    [Fact]
    public void Multiple_inputs()
    {
        var session = Ffmpeg
            .Input(new Uri("https://example.com/v.mp4"))
            .Input("/subs.srt")
            .Output("out.mkv", o => o
                .Map(0, 0, StreamType.Video, VideoCodec.Copy)
                .Map(1, 0, StreamType.Subtitle, "srt"));

        Assert.NotNull(session);
    }

    [Fact]
    public void Custom_binary_path()
    {
        var session = Ffmpeg
            .At("/usr/local/bin/ffmpeg")
            .Input(new Uri("https://example.com/v.mp4"))
            .Output("out.mkv", o => o
                .Map(0, 0, StreamType.Video, VideoCodec.Copy));

        Assert.NotNull(session);
    }

    [Fact]
    public void Output_metadata()
    {
        var session = Ffmpeg
            .Input(new Uri("https://example.com/v.mp4"))
            .Output("out.mkv", o => o
                .Map(0, 0, StreamType.Video, VideoCodec.Copy)
                .Metadata("s:s:0", "language=deu")
                .Arg("-bsf:a aac_adtstoasc"));

        Assert.NotNull(session);
    }
}
