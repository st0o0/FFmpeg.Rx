using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Filters;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Filters;

public sealed class ScaleFilterTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Scale_down_resolution()
    {
        var output = OutputPath("scaled.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                            Quality: Quality.Crf(28), Preset: Preset.UltraFast)))
                ],
                Filters: new SimpleFilterSpec(
                    VideoFilters: [new ScaleFilter(160, 120)]))]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        await FfmpegAssert.HasResolution(output, 160, 120);
    }
}
