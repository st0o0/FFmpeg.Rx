namespace FFmpeg.Rx.Tests;

public sealed class GlobalConfigTests : IDisposable
{
    public GlobalConfigTests()
    {
        FfmpegOptions.Reset();
    }

    public void Dispose()
    {
        FfmpegOptions.Reset();
    }

    [Fact]
    public void Default_binary_is_ffmpeg()
    {
        var binary = FfmpegOptions.ResolveFfmpegBinary(null);
        Assert.NotNull(binary);
    }

    [Fact]
    public void Per_session_overrides_global()
    {
        FfmpegOptions.Configure(new FfmpegOptions { BinaryPath = "/global/ffmpeg" });
        var binary = FfmpegOptions.ResolveFfmpegBinary("/session/ffmpeg");
        Assert.Equal("/session/ffmpeg", binary);
    }

    [Fact]
    public void Global_config_used_when_no_override()
    {
        FfmpegOptions.Configure(new FfmpegOptions { BinaryPath = "/global/ffmpeg" });
        var binary = FfmpegOptions.ResolveFfmpegBinary(null);
        Assert.Equal("/global/ffmpeg", binary);
    }

    [Fact]
    public void Ffprobe_binary_resolution()
    {
        FfmpegOptions.Configure(new FfmpegOptions { FfprobeBinaryPath = "/custom/ffprobe" });
        var binary = FfmpegOptions.ResolveFfprobeBinary(null);
        Assert.Equal("/custom/ffprobe", binary);
    }

    [Fact]
    public void Options_record_properties()
    {
        var opts = new FfmpegOptions
        {
            BinaryPath = "/bin/ffmpeg",
            FfprobeBinaryPath = "/bin/ffprobe",
            TempDirectory = "/tmp",
            WorkingDirectory = "/work"
        };

        Assert.Equal("/bin/ffmpeg", opts.BinaryPath);
        Assert.Equal("/bin/ffprobe", opts.FfprobeBinaryPath);
        Assert.Equal("/tmp", opts.TempDirectory);
        Assert.Equal("/work", opts.WorkingDirectory);
    }
}
