using FFmpeg.Rx.Events;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx.IntegrationTests.PipeIO;

public sealed class PipeProgressIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Pipe_output_with_progress_via_stderr()
    {
        using var outputStream = new MemoryStream();
        var progressEvents = new List<ProgressEvent>();

        var spec = new PipelineSpec(
            [new FileInputSpec(TestAsset("audio_only_3s.m4a"))],
            [new OutputSpec(
                new PipeDestination(outputStream),
                CustomArgs: ["-f s16le", "-acodec pcm_s16le", "-ar 44100", "-ac 1"])],
            GlobalOptions: ["-progress pipe:1"]);

        await using var session = FfmpegSession.Create(spec);
        session.Progress.Subscribe(progressEvents.Add);

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        Assert.True(outputStream.Length > 0, "Output stream should have data");
    }
}
