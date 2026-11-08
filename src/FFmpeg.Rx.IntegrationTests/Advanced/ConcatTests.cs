using FFmpeg.Rx.IntegrationTests.Internal;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class ConcatTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Concat_joins_two_files()
    {
        var output = OutputPath("joined.mp4");
        var input = TestAsset("video_h264_aac_5s.mp4");

        var result = await Ffmpeg.Concat([input, input], output, TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasDurationApprox(output, TimeSpan.FromSeconds(10), 1.0);
    }
}
