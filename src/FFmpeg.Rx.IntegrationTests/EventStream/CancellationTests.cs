using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.EventStream;

public sealed class CancellationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Cancel_during_transcode()
    {
        var output = OutputPath("out.mp4");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("video_h264_aac_5s.mp4")) { Options = new InputOptionConfig(StreamLoop: 10) }],
            [new OutputConfig(
                new FileDestination(output),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                            Quality: Quality.Crf(28), Preset: Preset.VerySlow)))
                ])]));

        cts.CancelAfter(TimeSpan.FromMilliseconds(500));

        try
        {
            var result = await session.RunAsync(cts.Token);
            Assert.False(result.Success);
        }
        catch (OperationCanceledException)
        {
            // Expected — cancellation propagated from ReadLineAsync
        }
    }
}
