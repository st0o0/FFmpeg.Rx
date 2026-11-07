using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class HWAccelTests
{
    private static string BuildWithInput(Action<Builder.InputOptions> configure)
    {
        var options = new Builder.InputOptions();
        configure(options);
        var pipeline = new FfmpegPipeline(
            [new FileInput("input.mp4", options.Arguments)],
            new FfmpegOutput("out.mp4", [], []),
            []);
        return ArgumentBuilder.Build(pipeline);
    }

    [Fact]
    public void HWAccel_constants()
    {
        Assert.Equal("cuda", HWAccel.Cuda);
        Assert.Equal("qsv", HWAccel.Qsv);
        Assert.Equal("dxva2", HWAccel.Dxva2);
        Assert.Equal("d3d11va", HWAccel.D3d11va);
        Assert.Equal("videotoolbox", HWAccel.VideoToolbox);
        Assert.Equal("vaapi", HWAccel.Vaapi);
        Assert.Equal("auto", HWAccel.Auto);
    }

    [Fact]
    public void HWAccel_cuda()
    {
        var args = BuildWithInput(o => o.HWAccel(HWAccel.Cuda));
        var hwIdx = args.IndexOf("-hwaccel cuda", StringComparison.Ordinal);
        var iIdx = args.IndexOf("-i ", StringComparison.Ordinal);
        Assert.True(hwIdx >= 0 && hwIdx < iIdx);
    }

    [Fact]
    public void HWAccel_with_device()
    {
        var args = BuildWithInput(o => o.HWAccel(HWAccel.Cuda).HWAccelDevice("0"));
        Assert.Contains("-hwaccel cuda", args);
        Assert.Contains("-hwaccel_device 0", args);
    }

    [Fact]
    public void HWAccel_output_format()
    {
        var args = BuildWithInput(o => o.HWAccelOutputFormat("cuda"));
        Assert.Contains("-hwaccel_output_format cuda", args);
    }

    [Fact]
    public void Video_decoder()
    {
        var args = BuildWithInput(o => o.VideoDecoder("h264_cuvid"));
        Assert.Contains("-c:v h264_cuvid", args);
        var cvIdx = args.IndexOf("-c:v h264_cuvid", StringComparison.Ordinal);
        var iIdx = args.IndexOf("-i ", StringComparison.Ordinal);
        Assert.True(cvIdx < iIdx);
    }

    [Fact]
    public void HW_encoder_constants()
    {
        Assert.Equal("h264_nvenc", VideoCodec.H264Nvenc);
        Assert.Equal("hevc_nvenc", VideoCodec.HevcNvenc);
        Assert.Equal("h264_qsv", VideoCodec.H264Qsv);
        Assert.Equal("hevc_qsv", VideoCodec.HevcQsv);
        Assert.Equal("h264_amf", VideoCodec.H264Amf);
        Assert.Equal("hevc_amf", VideoCodec.HevcAmf);
    }
}
