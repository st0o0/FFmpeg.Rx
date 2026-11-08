using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.IntegrationTests.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.IntegrationTests.Advanced;

public sealed class HWAccelIntegrationTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task HW_encode_nvenc_or_skip()
    {
        var output = OutputPath("hw.mp4");

        await using var session = FfmpegSession.Create(new PipelineSpec(
            [new FileInputSpec(TestAsset("video_h264_aac_5s.mp4"))],
            [new OutputSpec(
                new FileDestination(output),
                Streams: [new StreamSpec(0, 0, StreamType.Video,
                    new VideoEncode(VideoCodec.H264Nvenc, new VideoEncodingOptions()))])]));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        if (!result.Success && result.Error?.Contains("No NVENC capable devices found", StringComparison.OrdinalIgnoreCase) == true)
        {
            Assert.Skip("NVENC not available");
        }

        if (!result.Success && result.Error?.Contains("Unknown encoder", StringComparison.OrdinalIgnoreCase) == true)
        {
            Assert.Skip("NVENC encoder not available in this ffmpeg build");
        }

        Assert.True(result.Success, result.Error);
    }
}
