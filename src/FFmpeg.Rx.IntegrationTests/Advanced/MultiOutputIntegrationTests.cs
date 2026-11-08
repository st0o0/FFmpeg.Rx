using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class MultiOutputIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Two_outputs_different_quality()
    {
        var hq = OutputPath("hq.mp4");
        var lq = OutputPath("lq.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [
                new OutputSpec(
                    new FileDestination(hq),
                    Streams: [new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                            Quality: Quality.Crf(18), Preset: Preset.UltraFast)))]),
                new OutputSpec(
                    new FileDestination(lq),
                    Streams: [new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                            Quality: Quality.Crf(38), Preset: Preset.UltraFast)))])
            ]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(hq);
        FfmpegAssert.OutputExists(lq);
        Assert.True(new FileInfo(hq).Length > new FileInfo(lq).Length,
            "HQ file should be larger than LQ");
    }
}
