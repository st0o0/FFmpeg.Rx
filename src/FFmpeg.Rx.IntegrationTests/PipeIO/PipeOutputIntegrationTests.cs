using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.PipeIO;

public sealed class PipeOutputIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Pipe_output_to_stream()
    {
        using var outputStream = new MemoryStream();

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("audio_only_3s.m4a"))],
            [new OutputSpec(
                new PipeDestination(outputStream),
                CustomArgs: ["-f s16le", "-acodec pcm_s16le", "-ar 44100", "-ac 1"])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        Assert.True(outputStream.Length > 0, "Output stream should have data");
    }
}
