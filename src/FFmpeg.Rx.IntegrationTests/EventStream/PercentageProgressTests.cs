using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Events;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx.IntegrationTests.EventStream;

public sealed class PercentageProgressTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task WithProgressReporting_emits_percentage_events()
    {
        var output = OutputPath("out.mp4");
        var percentEvents = new List<PercentageProgressEvent>();

        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .WithProgressReporting()
            .Output(output, o => o
                .Stream(0, 0, StreamType.Video, s => s
                    .Encode(VideoCodec.H264, e => e
                        .Quality(Quality.Crf(28))
                        .Preset(Preset.UltraFast))));

        session.Events.OfType<FfmpegEvent, PercentageProgressEvent>().Subscribe(percentEvents.Add);

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        Assert.NotEmpty(percentEvents);
        Assert.True(percentEvents[^1].Percent > 0, "Last percentage should be > 0");
    }
}
