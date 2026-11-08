using FFmpeg.Rx.Codecs;

namespace FFmpeg.Rx.Tests;

public sealed class ValueObjectTests
{
    [Fact]
    public void VideoCodec_H264_name() => Assert.Equal("libx264", VideoCodec.H264.Name);

    [Fact]
    public void VideoCodec_Copy_name() => Assert.Equal("copy", VideoCodec.Copy.Name);

    [Fact]
    public void VideoCodec_Custom_factory()
    {
        var codec = VideoCodec.Custom("libsvtav1");
        Assert.Equal("libsvtav1", codec.Name);
    }

    [Fact]
    public void VideoCodec_equality()
    {
        Assert.Equal(VideoCodec.H264, VideoCodec.Custom("libx264"));
        Assert.NotEqual(VideoCodec.H264, VideoCodec.Hevc);
    }

    [Fact]
    public void AudioCodec_Aac_name() => Assert.Equal("aac", AudioCodec.Aac.Name);

    [Fact]
    public void AudioCodec_Copy_name() => Assert.Equal("copy", AudioCodec.Copy.Name);

    [Fact]
    public void AudioCodec_Custom_factory()
    {
        var codec = AudioCodec.Custom("libfdk_aac");
        Assert.Equal("libfdk_aac", codec.Name);
    }

    [Fact]
    public void SubtitleCodec_MovText_name() => Assert.Equal("mov_text", SubtitleCodec.MovText.Name);

    [Fact]
    public void SubtitleCodec_equality() => Assert.Equal(SubtitleCodec.Srt, SubtitleCodec.Custom("srt"));

    [Fact]
    public void Bitrate_Kbps_value()
    {
        var bitrate = Bitrate.Kbps(192);
        Assert.Equal(192000, bitrate.BitsPerSecond);
    }

    [Fact]
    public void Bitrate_Kbps_ffmpeg_string() => Assert.Equal("192k", Bitrate.Kbps(192).ToFfmpegString());

    [Fact]
    public void Bitrate_Mbps_value()
    {
        var bitrate = Bitrate.Mbps(4);
        Assert.Equal(4_000_000, bitrate.BitsPerSecond);
    }

    [Fact]
    public void Bitrate_Mbps_ffmpeg_string() => Assert.Equal("4M", Bitrate.Mbps(4).ToFfmpegString());

    [Fact]
    public void Bitrate_equality() => Assert.Equal(Bitrate.Kbps(1000), Bitrate.Mbps(1));

    [Fact]
    public void Framerate_Fps_int()
    {
        var fps = Framerate.Fps(30);
        Assert.Equal(30, fps.Numerator);
        Assert.Equal(1, fps.Denominator);
        Assert.Equal("30", fps.ToFfmpegString());
    }

    [Fact]
    public void Framerate_Rational()
    {
        var fps = Framerate.Rational(30000, 1001);
        Assert.Equal(30000, fps.Numerator);
        Assert.Equal(1001, fps.Denominator);
        Assert.Equal("30000/1001", fps.ToFfmpegString());
    }

    [Fact]
    public void Framerate_Ntsc30_equals_rational() => Assert.Equal(Framerate.Ntsc30, Framerate.Rational(30000, 1001));

    [Fact]
    public void Framerate_ToDouble()
    {
        var fps = Framerate.Ntsc30;
        Assert.InRange(fps.ToDouble(), 29.96, 29.98);
    }

    [Fact]
    public void QualitySpec_Crf_factory()
    {
        var quality = Quality.Crf(23);
        var cq = Assert.IsType<ConstantQuality>(quality);
        Assert.Equal(23, cq.Crf);
    }

    [Fact]
    public void QualitySpec_Bitrate_factory()
    {
        var quality = Quality.Bitrate(Bitrate.Mbps(4));
        var cb = Assert.IsType<ConstantBitrate>(quality);
        Assert.Equal(4_000_000, cb.Rate.BitsPerSecond);
    }

    [Fact]
    public void Preset_Slow_name() => Assert.Equal("slow", Preset.Slow.Name);

    [Fact]
    public void Preset_Custom_factory() => Assert.Equal("p5", Preset.Custom("p5").Name);

    [Fact]
    public void VideoProfile_High_name() => Assert.Equal("high", VideoProfile.High.Name);

    [Fact]
    public void Tune_Film_name() => Assert.Equal("film", Tune.Film.Name);

    [Fact]
    public void PixelFormat_Yuv420p_name() => Assert.Equal("yuv420p", PixelFormat.Yuv420p.Name);

    [Fact]
    public void ContainerFormat_Mp4_name() => Assert.Equal("mp4", ContainerFormat.Mp4.Name);

    [Fact]
    public void ContainerFormat_Null_name() => Assert.Equal("null", ContainerFormat.Null.Name);

    [Fact]
    public void ContainerFormat_Mkv_name() => Assert.Equal("matroska", ContainerFormat.Mkv.Name);

    [Fact]
    public void HWAccel_Cuda_name() => Assert.Equal("cuda", HWAccel.Cuda.Name);

    [Fact]
    public void HWAccel_Custom_factory() => Assert.Equal("vulkan", HWAccel.Custom("vulkan").Name);

    [Fact]
    public void VideoCodec_type_safety()
    {
        var video = VideoCodec.H264;
        var audio = AudioCodec.Aac;
        Assert.NotEqual(video.Name, audio.Name);
    }

    [Fact]
    public void All_HWAccel_constants()
    {
        Assert.Equal("auto", HWAccel.Auto.Name);
        Assert.Equal("cuda", HWAccel.Cuda.Name);
        Assert.Equal("qsv", HWAccel.Qsv.Name);
        Assert.Equal("dxva2", HWAccel.Dxva2.Name);
        Assert.Equal("d3d11va", HWAccel.D3d11va.Name);
        Assert.Equal("videotoolbox", HWAccel.VideoToolbox.Name);
        Assert.Equal("vaapi", HWAccel.Vaapi.Name);
    }

    [Fact]
    public void All_VideoCodec_constants()
    {
        Assert.Equal("libx264", VideoCodec.H264.Name);
        Assert.Equal("libx265", VideoCodec.Hevc.Name);
        Assert.Equal("libvpx-vp9", VideoCodec.Vp9.Name);
        Assert.Equal("libaom-av1", VideoCodec.Av1.Name);
        Assert.Equal("h264_nvenc", VideoCodec.H264Nvenc.Name);
        Assert.Equal("hevc_nvenc", VideoCodec.HevcNvenc.Name);
        Assert.Equal("h264_qsv", VideoCodec.H264Qsv.Name);
        Assert.Equal("hevc_qsv", VideoCodec.HevcQsv.Name);
        Assert.Equal("copy", VideoCodec.Copy.Name);
    }

    [Fact]
    public void All_Preset_constants()
    {
        Assert.Equal("ultrafast", Preset.UltraFast.Name);
        Assert.Equal("superfast", Preset.SuperFast.Name);
        Assert.Equal("veryfast", Preset.VeryFast.Name);
        Assert.Equal("faster", Preset.Faster.Name);
        Assert.Equal("fast", Preset.Fast.Name);
        Assert.Equal("medium", Preset.Medium.Name);
        Assert.Equal("slow", Preset.Slow.Name);
        Assert.Equal("slower", Preset.Slower.Name);
        Assert.Equal("veryslow", Preset.VerySlow.Name);
    }
}
