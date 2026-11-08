using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Transcode;

public sealed class BasicTranscodeTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Transcode_H264_with_Crf()
    {
        var output = OutputPath("out.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video,
                        new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Quality: Quality.Crf(28)))),
                    new StreamSpec(0, 0, StreamType.Audio,
                        new AudioEncode(AudioCodec.Aac, new AudioEncodingOptions(Bitrate: Bitrate.Kbps(64))))
                ])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasVideoStream(output, "h264");
        await FfmpegAssert.HasAudioStream(output, "aac");
        await FfmpegAssert.HasDurationApprox(output, TimeSpan.FromSeconds(5));
    }

    [Fact(Timeout = 30000)]
    public async Task Stream_copy_remux_Mp4_to_Mkv()
    {
        var output = OutputPath("out.mkv");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())
                ])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasVideoStream(output, "h264");
        await FfmpegAssert.HasAudioStream(output, "aac");
    }

    [Fact(Timeout = 30000)]
    public async Task Format_conversion_with_container()
    {
        var output = OutputPath("out.ts");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [
                    new StreamSpec(0, 0, StreamType.Video, new StreamCopy()),
                    new StreamSpec(0, 0, StreamType.Audio, new StreamCopy())
                ],
                Container: ContainerFormat.Ts)]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
    }
}
