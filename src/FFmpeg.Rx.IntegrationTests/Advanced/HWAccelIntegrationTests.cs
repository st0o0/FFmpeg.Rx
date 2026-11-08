using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class HWAccelIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Encode_with_alternative_software_codec()
    {
        var output = OutputPath("hevc.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [new StreamSpec(0, 0, StreamType.Video,
                    new VideoEncode(VideoCodec.Hevc, new VideoEncodingOptions()))])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);

        var info = await Ffmpeg.Probe(output, TestContext.Current.CancellationToken);
        Assert.Equal("hevc", info.PrimaryVideo?.Codec);
    }

    [Fact(Timeout = 30000)]
    public async Task HW_encode_nvenc_or_skip()
    {
        await SkipIfEncoderUnavailable("h264_nvenc");

        var output = OutputPath("hw.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [new StreamSpec(0, 0, StreamType.Video,
                    new VideoEncode(VideoCodec.H264Nvenc, new VideoEncodingOptions()))])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
    }

    private static async Task SkipIfEncoderUnavailable(string encoderName)
    {
        var sb = new System.Text.StringBuilder();

        await CliWrap.Cli.Wrap("ffmpeg")
            .WithArguments(["-encoders"])
            .WithValidation(CliWrap.CommandResultValidation.None)
            .WithStandardOutputPipe(CliWrap.PipeTarget.ToStringBuilder(sb))
            .ExecuteAsync();

        if (!sb.ToString().Contains(encoderName, StringComparison.Ordinal))
        {
            Assert.Skip($"Encoder '{encoderName}' not available in this ffmpeg build");
        }
    }
}
