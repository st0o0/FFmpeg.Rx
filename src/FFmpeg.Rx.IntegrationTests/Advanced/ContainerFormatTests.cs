using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class ContainerFormatTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Output_to_Mkv()
    {
        var output = OutputPath("out.mkv");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputConfig(
                new FileDestination(output),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamConfig(0, 0, StreamType.Audio, new StreamCopy())
                ],
                Container: ContainerFormat.Mkv)]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);

        var info = await Ffprobe.AnalyseAsync(output, TestContext.Current.CancellationToken);
        Assert.Contains("matroska", info.FormatName, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(Timeout = 30000)]
    public async Task Output_to_Ts()
    {
        var output = OutputPath("out.ts");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputConfig(
                new FileDestination(output),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamConfig(0, 0, StreamType.Audio, new StreamCopy())
                ],
                Container: ContainerFormat.Ts)]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);

        var info = await Ffprobe.AnalyseAsync(output, TestContext.Current.CancellationToken);
        Assert.Contains("mpegts", info.FormatName, StringComparison.OrdinalIgnoreCase);
    }
}
