using FFmpeg.Rx.Events;
using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Probing;
using R3;

namespace FFmpeg.Rx.Tests;

public sealed class ErrorPathTests
{
    [Fact(Timeout = 15000)]
    public async Task Probe_nonexistent_binary_throws_FfprobeException()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var ex = await Assert.ThrowsAsync<FfprobeException>(() =>
                Ffprobe.AnalyseAsync(tempFile, new FfprobeOptions { BinaryPath = "nonexistent_ffprobe_12345" }, TestContext.Current.CancellationToken));
            Assert.Contains("Failed to start", ex.Message);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact(Timeout = 5000)]
    public async Task Probe_nonexistent_file_throws_FileNotFoundException()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            Ffprobe.AnalyseAsync("/nonexistent/path/video.mp4", TestContext.Current.CancellationToken));
    }

    [Fact]
    public void FfprobeException_has_exit_code_and_stderr()
    {
        var ex = new FfprobeException(1, "some error");
        Assert.Equal(1, ex.ExitCode);
        Assert.Equal("some error", ex.StandardError);
        Assert.Contains("some error", ex.Message);
    }

    [Fact]
    public void ErrorClassifier_empty_is_permanent()
    {
        Assert.Equal(FailureKind.Permanent, ErrorClassifier.Classify(null));
        Assert.Equal(FailureKind.Permanent, ErrorClassifier.Classify(""));
        Assert.Equal(FailureKind.Permanent, ErrorClassifier.Classify("   "));
    }

    [Fact]
    public void ErrorClassifier_502_is_transient()
    {
        Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify("HTTP 502 Bad Gateway"));
    }

    [Fact]
    public void ErrorClassifier_503_is_transient()
    {
        Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify("HTTP 503 Service Unavailable"));
    }

    [Fact]
    public void ErrorClassifier_connection_reset_is_transient()
    {
        Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify("Connection reset by peer"));
    }

    [Fact]
    public void ErrorClassifier_connection_refused_is_transient()
    {
        Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify("Connection refused"));
    }

    [Fact]
    public void ErrorClassifier_network_unreachable_is_transient()
    {
        Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify("Network is unreachable"));
    }

    [Fact]
    public void ErrorClassifier_temporary_failure_is_transient()
    {
        Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify("Temporary failure in name resolution"));
    }

    [Fact]
    public void ErrorClassifier_timed_out_is_transient()
    {
        Assert.Equal(FailureKind.Transient, ErrorClassifier.Classify("Connection timed out"));
    }

    [Fact]
    public void ErrorClassifier_unknown_error_is_permanent()
    {
        Assert.Equal(FailureKind.Permanent, ErrorClassifier.Classify("No such file or directory"));
    }

    [Fact]
    public void ExtractError_empty_returns_empty()
    {
        Assert.Equal("", ErrorClassifier.ExtractError(null));
        Assert.Equal("", ErrorClassifier.ExtractError(""));
    }

    [Fact]
    public void ExtractError_finds_http_error()
    {
        var stderr = "line1\nHTTP error 404 Not Found\nline3";
        Assert.Equal("HTTP error 404 Not Found", ErrorClassifier.ExtractError(stderr));
    }

    [Fact]
    public void ExtractError_finds_invalid_data()
    {
        var stderr = "info line\nInvalid data found when processing input\n";
        Assert.Equal("Invalid data found when processing input", ErrorClassifier.ExtractError(stderr));
    }

    [Fact]
    public void ExtractError_falls_back_to_last_line()
    {
        var stderr = "some warning\nactual error message";
        Assert.Equal("actual error message", ErrorClassifier.ExtractError(stderr));
    }

    [Fact]
    public void FfmpegOptions_configure_and_reset()
    {
        FfmpegOptions.Configure(new FfmpegOptions { BinaryPath = "/custom/ffmpeg" });
        Assert.NotNull(FfmpegOptions.Global);
        Assert.Equal("/custom/ffmpeg", FfmpegOptions.Global.BinaryPath);
        FfmpegOptions.Reset();
        Assert.Null(FfmpegOptions.Global);
    }

    [Fact]
    public void FfmpegOptions_resolve_per_session_override()
    {
        var result = FfmpegOptions.ResolveFfmpegBinary("/override/ffmpeg");
        Assert.Equal("/override/ffmpeg", result);
    }

    [Fact]
    public void FfmpegOptions_resolve_global_override()
    {
        FfmpegOptions.Configure(new FfmpegOptions { BinaryPath = "/global/ffmpeg" });
        try
        {
            var result = FfmpegOptions.ResolveFfmpegBinary(null);
            Assert.Equal("/global/ffmpeg", result);
        }
        finally
        {
            FfmpegOptions.Reset();
        }
    }

    [Fact]
    public void AsPercentage_calculates_correctly()
    {
        var progress = new ProgressEvent(0, 2_500_000, 1.0);
        var percent = progress.AsPercentage(TimeSpan.FromSeconds(5));
        Assert.Equal(50.0, percent, 0.1);
    }

    [Fact]
    public void AsPercentage_zero_duration_returns_zero()
    {
        var progress = new ProgressEvent(0, 1_000_000, 1.0);
        var percent = progress.AsPercentage(TimeSpan.Zero);
        Assert.Equal(0.0, percent);
    }

    [Fact]
    public void AsPercentage_clamps_to_100()
    {
        var progress = new ProgressEvent(0, 10_000_000, 1.0);
        var percent = progress.AsPercentage(TimeSpan.FromSeconds(5));
        Assert.Equal(100.0, percent);
    }

    [Fact]
    public void WithPercentage_emits_PercentageProgressEvent()
    {
        var progress = new ProgressEvent(0, 2_500_000, 1.0);
        var events = new List<PercentageProgressEvent>();

        var subject = new Subject<ProgressEvent>();
        subject
            .WithPercentage(TimeSpan.FromSeconds(5))
            .Subscribe(events.Add);

        subject.OnNext(progress);
        subject.OnCompleted();
        subject.Dispose();

        Assert.Single(events);
        Assert.Equal(50.0, events[0].Percent, 0.1);
        Assert.Same(progress, events[0].Source);
    }
}
