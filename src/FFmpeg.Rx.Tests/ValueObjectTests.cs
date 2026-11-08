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
        Assert.Equal("libsvtav1", VideoCodec.SvtAv1.Name);
        Assert.Equal("h264_nvenc", VideoCodec.H264Nvenc.Name);
        Assert.Equal("hevc_nvenc", VideoCodec.HevcNvenc.Name);
        Assert.Equal("h264_qsv", VideoCodec.H264Qsv.Name);
        Assert.Equal("hevc_qsv", VideoCodec.HevcQsv.Name);
        Assert.Equal("h264_amf", VideoCodec.H264Amf.Name);
        Assert.Equal("hevc_amf", VideoCodec.HevcAmf.Name);
        Assert.Equal("h264_videotoolbox", VideoCodec.H264VideoToolbox.Name);
        Assert.Equal("hevc_videotoolbox", VideoCodec.HevcVideoToolbox.Name);
        Assert.Equal("h264_cuvid", VideoCodec.H264Cuvid.Name);
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

    [Fact]
    public void All_AudioCodec_constants()
    {
        Assert.Equal("aac", AudioCodec.Aac.Name);
        Assert.Equal("libopus", AudioCodec.Opus.Name);
        Assert.Equal("libmp3lame", AudioCodec.Mp3.Name);
        Assert.Equal("flac", AudioCodec.Flac.Name);
        Assert.Equal("pcm_s16le", AudioCodec.Pcm16Le.Name);
        Assert.Equal("pcm_s24le", AudioCodec.Pcm24Le.Name);
        Assert.Equal("libvorbis", AudioCodec.Vorbis.Name);
        Assert.Equal("copy", AudioCodec.Copy.Name);
    }

    [Fact]
    public void All_SubtitleCodec_constants()
    {
        Assert.Equal("mov_text", SubtitleCodec.MovText.Name);
        Assert.Equal("srt", SubtitleCodec.Srt.Name);
        Assert.Equal("ass", SubtitleCodec.Ass.Name);
        Assert.Equal("webvtt", SubtitleCodec.WebVtt.Name);
        Assert.Equal("copy", SubtitleCodec.Copy.Name);
    }

    [Fact]
    public void All_PixelFormat_constants()
    {
        Assert.Equal("yuv420p", PixelFormat.Yuv420p.Name);
        Assert.Equal("yuv422p", PixelFormat.Yuv422p.Name);
        Assert.Equal("yuv444p", PixelFormat.Yuv444p.Name);
        Assert.Equal("rgb24", PixelFormat.Rgb24.Name);
        Assert.Equal("rgba", PixelFormat.Rgba.Name);
        Assert.Equal("nv12", PixelFormat.Nv12.Name);
        Assert.Equal("nv21", PixelFormat.Nv21.Name);
        Assert.Equal("p010le", PixelFormat.P010Le.Name);
    }

    [Fact]
    public void All_Tune_constants()
    {
        Assert.Equal("film", Tune.Film.Name);
        Assert.Equal("animation", Tune.Animation.Name);
        Assert.Equal("grain", Tune.Grain.Name);
        Assert.Equal("stillimage", Tune.StillImage.Name);
        Assert.Equal("fastdecode", Tune.FastDecode.Name);
        Assert.Equal("zerolatency", Tune.ZeroLatency.Name);
    }

    [Fact]
    public void All_VideoProfile_constants()
    {
        Assert.Equal("baseline", VideoProfile.Baseline.Name);
        Assert.Equal("main", VideoProfile.Main.Name);
        Assert.Equal("high", VideoProfile.High.Name);
        Assert.Equal("high10", VideoProfile.High10.Name);
        Assert.Equal("high422", VideoProfile.High422.Name);
        Assert.Equal("high444", VideoProfile.High444.Name);
    }

    [Fact]
    public void All_ContainerFormat_constants()
    {
        Assert.Equal("mp4", ContainerFormat.Mp4.Name);
        Assert.Equal("matroska", ContainerFormat.Mkv.Name);
        Assert.Equal("webm", ContainerFormat.WebM.Name);
        Assert.Equal("mpegts", ContainerFormat.Ts.Name);
        Assert.Equal("flv", ContainerFormat.Flv.Name);
        Assert.Equal("avi", ContainerFormat.Avi.Name);
        Assert.Equal("mov", ContainerFormat.Mov.Name);
        Assert.Equal("ogg", ContainerFormat.Ogg.Name);
        Assert.Equal("wav", ContainerFormat.Wav.Name);
        Assert.Equal("mp3", ContainerFormat.Mp3.Name);
        Assert.Equal("flac", ContainerFormat.Flac.Name);
        Assert.Equal("null", ContainerFormat.Null.Name);
    }

    [Fact]
    public void Framerate_Fps_double()
    {
        var fps = Framerate.Fps(29.97);
        Assert.InRange(fps.ToDouble(), 29.96, 29.98);
    }

    [Fact]
    public void Framerate_Ntsc24() => Assert.Equal(Framerate.Ntsc24, Framerate.Rational(24000, 1001));

    [Fact]
    public void Framerate_Ntsc60() => Assert.Equal(Framerate.Ntsc60, Framerate.Rational(60000, 1001));

    [Fact]
    public void Bitrate_Mbps_double()
    {
        var bitrate = Bitrate.Mbps(2.5);
        Assert.Equal(2_500_000, bitrate.BitsPerSecond);
    }

    [Fact]
    public void Bitrate_raw_bits_ffmpeg_string()
    {
        var bitrate = Bitrate.Kbps(1500);
        Assert.Equal("1500k", bitrate.ToFfmpegString());
    }
}
