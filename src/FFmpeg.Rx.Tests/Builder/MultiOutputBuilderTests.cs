using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class MultiOutputBuilderTests
{
    [Fact]
    public void Two_outputs_creates_session()
    {
        var session = Ffmpeg.Input("input.mp4")
            .Outputs()
            .Output("hd.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()))
            .Output("sd.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()))
            .Build();

        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }

    [Fact]
    public void Three_outputs()
    {
        var session = Ffmpeg.Input("input.mp4")
            .Outputs()
            .Output("a.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()))
            .Output("b.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()))
            .Output("c.mp4", o => o.Stream(0, 0, StreamType.Video, s => s.Copy()))
            .Build();

        Assert.IsAssignableFrom<IFfmpegSession>(session);
    }
}
