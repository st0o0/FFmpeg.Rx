using System.Diagnostics;
using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Events;
using FFmpeg.Rx.Models;
using R3;

namespace FFmpeg.Rx.Tests;

public sealed class FfmpegSessionIntegrationTests
{
    private static bool FfmpegAvailable()
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo("ffmpeg", "-version")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            });
            p?.WaitForExit(5000);
            return p?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    [Fact]
    public async Task Session_emits_started_and_terminal_event()
    {
        if (!FfmpegAvailable())
        {
            Assert.Skip("ffmpeg not found on PATH");
        }

        var events = new List<FfmpegEvent>();

        await using var session = Ffmpeg.Input(new Uri("https://example.com/nonexistent.mp4"))
            .Output("out.mkv", o => o
                .Map(0, 0, StreamType.Video, VideoCodec.Copy));

        session.Events.Subscribe(events.Add);

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.NotEmpty(events);
        Assert.IsType<StartedEvent>(events[0]);
        Assert.True(events[^1] is CompletedEvent or FailedEvent);
        Assert.Equal(events[^1] is CompletedEvent, result.Success);
    }

    [Fact]
    public async Task Double_run_throws()
    {
        if (!FfmpegAvailable())
        {
            Assert.Skip("ffmpeg not found on PATH");
        }

        await using var session = FfmpegSession.Create(
            new FfmpegPipeline(
                [new UrlInput(new Uri("https://example.com/v.mp4"))],
                new FfmpegOutput("out.mkv", [], []),
                []));

        _ = await session.RunAsync(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => session.RunAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Missing_binary_produces_failed_result()
    {
        await using var session = FfmpegSession.Create(
            new FfmpegPipeline(
                [new UrlInput(new Uri("https://example.com/v.mp4"))],
                new FfmpegOutput("out.mkv", [], []),
                [],
                BinaryPath: "nonexistent-ffmpeg-binary-12345"));

        var result = await session.RunAsync(TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Contains("Failed to start FFmpeg", result.Error);
    }
}
