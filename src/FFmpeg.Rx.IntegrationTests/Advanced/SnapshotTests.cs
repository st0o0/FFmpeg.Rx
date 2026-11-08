using FFmpeg.Rx.IntegrationTests.Internal;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class SnapshotTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Snapshot_extracts_frame()
    {
        var output = OutputPath("thumb.jpg");

        var result = await Ffmpeg.Snapshot(
            TestAsset("video_h264_aac_5s.mp4"),
            TimeSpan.FromSeconds(2),
            output,
            TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
    }
}
