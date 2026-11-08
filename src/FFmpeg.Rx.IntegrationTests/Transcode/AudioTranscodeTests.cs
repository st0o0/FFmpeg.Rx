using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Transcode;

public sealed class AudioTranscodeTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Transcode_audio_bitrate()
    {
        var output = OutputPath("out.mp4");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputConfig(
                new FileDestination(output),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamConfig(0, 0, StreamType.Audio,
                        new AudioEncode(AudioCodec.Aac, new AudioEncodingOptions(Bitrate: Bitrate.Kbps(64))))
                ])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasAudioStream(output, "aac");
    }

    [Fact(Timeout = 30000)]
    public async Task Audio_only_transcode()
    {
        var output = OutputPath("out.m4a");

        await using var session = FfmpegSession.Create(new PipelineConfig(
            [new FileInputConfig(TestAsset("audio_only_3s.m4a"))],
            [new OutputConfig(
                new FileDestination(output),
                Streams: [
                    new StreamConfig(0, 0, StreamType.Audio,
                        new AudioEncode(AudioCodec.Aac, new AudioEncodingOptions(Bitrate: Bitrate.Kbps(32))))
                ])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasDurationApprox(output, TimeSpan.FromSeconds(3));
    }
}
