using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.PipeIO;

public sealed class PipeInputIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Pipe_raw_audio_input()
    {
        var rawOutput = OutputPath("raw.pcm");
        var finalOutput = OutputPath("from_pipe.m4a");

        await using var extract = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("audio_only_3s.m4a"))],
            [new OutputSpec(
                new FileDestination(rawOutput),
                CustomArgs: ["-f s16le", "-acodec pcm_s16le", "-ar 44100", "-ac 1"])]));

        var extractResult = await extract.RunAsync(TestContext.Current.CancellationToken);
        Assert.True(extractResult.Success, extractResult.Error);

        await using var rawStream = File.OpenRead(rawOutput);
        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new PipeInputSpec(rawStream, "s16le") { Options = new InputOptionSpec(CustomArgs: ["-ar 44100", "-ac 1"]) }],
            [new OutputSpec(
                new FileDestination(finalOutput),
                Streams: [new StreamSpec(0, 0, StreamType.Audio,
                    new AudioEncode(AudioCodec.Aac, new AudioEncodingOptions(Bitrate: Bitrate.Kbps(64))))])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(finalOutput);
    }
}
