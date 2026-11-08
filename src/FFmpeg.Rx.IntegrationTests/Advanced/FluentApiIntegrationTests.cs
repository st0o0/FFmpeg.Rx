using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

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

    [Fact(Timeout = 30000)]
    public async Task Fluent_with_encoding_options()
    {
        var output = OutputPath("encoded.mp4");

        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .Output(output, o => o
                .Stream(0, 0, StreamType.Video, s => s
                    .Encode(VideoCodec.H264, e => e
                        .Quality(Quality.Crf(28))
                        .Preset(Preset.UltraFast)
                        .Profile(VideoProfile.High)
                        .PixelFormat(Codecs.PixelFormat.Yuv420p)))
                .Stream(0, 0, StreamType.Audio, s => s
                    .Encode(AudioCodec.Aac, e => e
                        .Bitrate(Bitrate.Kbps(96))
                        .Channels(2))));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasVideoStream(output, "h264");
        await FfmpegAssert.HasAudioStream(output, "aac");
    }

    [Fact(Timeout = 30000)]
    public async Task Fluent_with_time_range()
    {
        var output = OutputPath("trimmed.mp4");

        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .Output(output, o => o
                .Stream(0, 0, StreamType.Video, s => s.Copy())
                .Stream(0, 0, StreamType.Audio, s => s.Copy())
                .TimeRange(t => t.Seek(TimeSpan.FromSeconds(1)).Duration(TimeSpan.FromSeconds(2))));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasDurationApprox(output, TimeSpan.FromSeconds(2), 0.5);
    }

    [Fact(Timeout = 30000)]
    public async Task Fluent_with_metadata()
    {
        var output = OutputPath("meta.mp4");

        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .Output(output, o => o
                .Stream(0, 0, StreamType.Video, s => s.Copy())
                .Stream(0, 0, StreamType.Audio, s => s.Copy())
                .Metadata(m => m.Global("title", "Test").Global("artist", "FFmpeg.Rx")));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
    }

    [Fact(Timeout = 30000)]
    public async Task Fluent_null_output()
    {
        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .OutputNull(o => o
                .Stream(0, 0, StreamType.Video, s => s.Copy()));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
    }

    [Fact(Timeout = 30000)]
    public async Task Fluent_with_video_filter()
    {
        var output = OutputPath("filtered.mp4");

        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .Output(output, o => o
                .Stream(0, 0, StreamType.Video, s => s
                    .Encode(VideoCodec.H264, e => e
                        .Quality(Quality.Crf(28))
                        .Preset(Preset.UltraFast)))
                .VideoFilter(f => f.Scale(160, 120)));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
        await FfmpegAssert.HasResolution(output, 160, 120);
    }

    [Fact(Timeout = 30000)]
    public async Task Fluent_with_container_format()
    {
        var output = OutputPath("out.ts");

        await using var session = Ffmpeg.Input(TestAsset("video_h264_aac_5s.mp4"))
            .Output(output, o => o
                .Stream(0, 0, StreamType.Video, s => s.Copy())
                .Stream(0, 0, StreamType.Audio, s => s.Copy())
                .Container(ContainerFormat.Ts));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.True(result.Success, result.Error);
        FfmpegAssert.OutputExists(output);
    }
}
