using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class MetadataTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Strip_all_metadata()
    {
        var output = OutputPath("stripped.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())
                ],
                Metadata: new MetadataSpec(StripAll: true))]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
    }

    [Fact(Timeout = 30000)]
    public async Task Add_global_metadata()
    {
        var output = OutputPath("titled.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())
                ],
                Metadata: new MetadataSpec(
                    GlobalEntries: [new("title", "Test Title")]))]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
    }
}
