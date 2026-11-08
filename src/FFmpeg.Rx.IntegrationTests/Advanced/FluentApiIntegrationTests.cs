using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class FluentApiIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Fluent_transcode_end_to_end()
    {
        var output = OutputPath("fluent.mp4");

        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .Output(output, o => o
                .Stream(0, 0, StreamType.Video, s => s
                    .Encode(VideoCodec.H264, e => e
                        .Quality(Quality.Crf(28))
                        .Preset(Preset.UltraFast)))
                .Stream(0, 0, StreamType.Audio, s => s.Copy()));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasVideoStream(output, "h264");
        await FfmpegAssert.HasAudioStream(output, "aac");
    }
}
