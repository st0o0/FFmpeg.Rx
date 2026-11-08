using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class NullOutputIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Null_output_completes()
    {
        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputConfig(NullDestination.Instance, Container: ContainerFormat.Null)]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
    }
}
