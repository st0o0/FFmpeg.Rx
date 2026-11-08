using FFmpeg.Rx.IntegrationTests.Internal;

namespace FFmpeg.Rx.IntegrationTests.Probing;

public sealed class ProbeVideoTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Probe_video_h264_aac()
    {
        var info = await Ffprobe.AnalyseAsync(TestAsset("video_h264_aac_5s.mp4"), TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.Single(info.VideoStreams);
        Assert.Single(info.AudioStreams);

        var video = info.VideoStreams[0];
        Assert.Contains("h264", video.Codec, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(320, video.Width);
        Assert.Equal(240, video.Height);

        var audio = info.AudioStreams[0];
        Assert.Contains("aac", audio.Codec, StringComparison.OrdinalIgnoreCase);

        Assert.InRange(info.Duration.TotalSeconds, 4.5, 5.5);
    }

    [Fact(Timeout = 30000)]
    public async Task Probe_hevc_video()
    {
        var info = await Ffprobe.AnalyseAsync(TestAsset("video_hevc_3s.mp4"), TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.Single(info.VideoStreams);
        Assert.Contains("hevc", info.VideoStreams[0].Codec, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(Timeout = 30000)]
    public async Task Probe_two_audio_tracks()
    {
        var info = await Ffprobe.AnalyseAsync(TestAsset("two_audio_tracks.mkv"), TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.Single(info.VideoStreams);
        Assert.Equal(2, info.AudioStreams.Count);
    }
}
