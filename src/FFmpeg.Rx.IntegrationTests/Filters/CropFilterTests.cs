using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Filters;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Filters;

public sealed class CropFilterTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Crop_to_square()
    {
        var output = OutputPath("cropped.mp4");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputConfig(
                new FileDestination(output),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                            Quality: Quality.Crf(28), Preset: Preset.UltraFast)))
                ],
                Filters: new SimpleFilterConfig(
                    VideoFilters: [new CropFilter(200, 200, 60, 20)]))]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        await FfmpegAssert.HasResolution(output, 200, 200);
    }
}
