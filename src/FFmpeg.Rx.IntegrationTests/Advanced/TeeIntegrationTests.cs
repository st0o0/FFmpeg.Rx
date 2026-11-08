using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class TeeIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Tee_split_two_files()
    {
        var out1 = OutputPath("tee1.mp4");
        var out2 = OutputPath("tee2.mp4");

        var teeOut1 = out1.Replace('\\', '/');
        var teeOut2 = out2.Replace('\\', '/');

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(""),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())
                ],
                Tee: new TeeOutput([
                    new TeeTarget(teeOut1, Format: "mp4"),
                    new TeeTarget(teeOut2, Format: "mp4")
                ]))]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(out1);
        FfmpegAssert.OutputExists(out2);
    }
}
