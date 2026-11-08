using FFmpeg.Rx.IntegrationTests.Internal;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class ProbeShortcutTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Probe_shortcut_returns_media_info()
    {
        var info = await Ffmpeg.Probe(
            TestAsset("video_h264_aac_5s.mp4"),
            TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.Single(info.VideoStreams);
        Assert.Single(info.AudioStreams);
    }
}
