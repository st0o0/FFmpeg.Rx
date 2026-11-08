using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Transcode;

public sealed class SubtitleMuxTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Mux_Srt_into_Mkv()
    {
        var output = OutputPath("out.mkv");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("video_h264_aac_5s.mp4")),
             new FileInputConfig(TestAsset("subtitle.srt"))],
            [new OutputConfig(
                new FileDestination(output),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamConfig(0, 0, StreamType.Audio, new StreamCopy()),
                    new StreamConfig(1, 0, StreamType.Subtitle, new SubtitleEncode(SubtitleCodec.Srt))
                ])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasSubtitleStream(output);
    }
}
