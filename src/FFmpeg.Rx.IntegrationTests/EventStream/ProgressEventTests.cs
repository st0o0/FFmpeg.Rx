using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Events;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx.IntegrationTests.EventStream;

public sealed class ProgressEventTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Successful_transcode_emits_correct_event_order()
    {
        var output = OutputPath("out.mp4");
        var events = new List<FfmpegEvent>();

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                            Quality: Quality.Crf(28), Preset: Preset.UltraFast)))
                ])],
            GlobalOptions: ["-progress pipe:1"]));

        session.Events.Subscribe(events.Add);
        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        Assert.NotEmpty(events);
        Assert.IsType<StartedEvent>(events[0]);
        Assert.IsType<CompletedEvent>(events[^1]);
    }

    [Fact(Timeout = 30000)]
    public async Task Progress_events_have_valid_data()
    {
        var output = OutputPath("out.mp4");
        var progressEvents = new List<ProgressEvent>();

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(
                            Quality: Quality.Crf(28), Preset: Preset.UltraFast)))
                ])],
            GlobalOptions: ["-progress pipe:1"]));

        session.Progress.Subscribe(progressEvents.Add);
        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        if (progressEvents.Count > 0)
        {
            var last = progressEvents[^1];
            Assert.True(last.BytesWritten > 0, "Last progress should have bytes written > 0");
        }
    }
}
