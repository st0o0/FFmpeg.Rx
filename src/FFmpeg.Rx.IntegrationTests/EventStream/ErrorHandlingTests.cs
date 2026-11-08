using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.EventStream;

public sealed class ErrorHandlingTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Corrupt_input_emits_FailedEvent()
    {
        var output = OutputPath("out.mp4");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("corrupt.mp4"))],
            [new OutputConfig(new FileDestination(output))]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    [Fact(Timeout = 30000)]
    public async Task Missing_input_file_fails()
    {
        var output = OutputPath("out.mp4");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig("/nonexistent/path/video.mp4")],
            [new OutputConfig(new FileDestination(output))]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.False(result.Success);
    }
}
