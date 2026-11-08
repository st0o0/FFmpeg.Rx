using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests;

public sealed class HWAccelTests
{
    private static string SerializeWithInput(InputOptionConfig opts)
    {
        var spec = new PipelineConfig(
            [new FileInputConfig("input.mp4") { Options = opts }],
            [new OutputConfig(new FileDestination("out.mp4"))]);
        return ArgumentSerializer.Serialize(spec);
    }

    [Fact]
    public void HWAccel_struct_names()
    {
        Assert.Equal("cuda", HWAccel.Cuda.Name);
        Assert.Equal("qsv", HWAccel.Qsv.Name);
        Assert.Equal("dxva2", HWAccel.Dxva2.Name);
        Assert.Equal("d3d11va", HWAccel.D3d11va.Name);
        Assert.Equal("videotoolbox", HWAccel.VideoToolbox.Name);
        Assert.Equal("vaapi", HWAccel.Vaapi.Name);
        Assert.Equal("auto", HWAccel.Auto.Name);
    }

    [Fact]
    public void HWAccel_cuda()
    {
        var args = SerializeWithInput(new InputOptionConfig(HWAccel: HWAccel.Cuda));
        var hwIdx = args.IndexOf("-hwaccel cuda", StringComparison.Ordinal);
        var iIdx = args.IndexOf("-i ", StringComparison.Ordinal);
        Assert.True(hwIdx >= 0 && hwIdx < iIdx);
    }

    [Fact]
    public void HWAccel_with_device()
    {
        var args = SerializeWithInput(new InputOptionConfig(HWAccel: HWAccel.Cuda, HWAccelDevice: "0"));
        Assert.Contains("-hwaccel cuda", args);
        Assert.Contains("-hwaccel_device 0", args);
    }

    [Fact]
    public void HWAccel_output_format()
    {
        var args = SerializeWithInput(new InputOptionConfig(HWAccelOutputFormat: "cuda"));
        Assert.Contains("-hwaccel_output_format cuda", args);
    }

    [Fact]
    public void Video_decoder()
    {
        var args = SerializeWithInput(new InputOptionConfig(VideoDecoder: VideoCodec.H264Cuvid));
        Assert.Contains("-c:v h264_cuvid", args);
        var cvIdx = args.IndexOf("-c:v h264_cuvid", StringComparison.Ordinal);
        var iIdx = args.IndexOf("-i ", StringComparison.Ordinal);
        Assert.True(cvIdx < iIdx);
    }

    [Fact]
    public void HW_encoder_struct_names()
    {
        Assert.Equal("h264_nvenc", VideoCodec.H264Nvenc.Name);
        Assert.Equal("hevc_nvenc", VideoCodec.HevcNvenc.Name);
        Assert.Equal("h264_qsv", VideoCodec.H264Qsv.Name);
        Assert.Equal("hevc_qsv", VideoCodec.HevcQsv.Name);
        Assert.Equal("h264_amf", VideoCodec.H264Amf.Name);
        Assert.Equal("hevc_amf", VideoCodec.HevcAmf.Name);
    }
}
