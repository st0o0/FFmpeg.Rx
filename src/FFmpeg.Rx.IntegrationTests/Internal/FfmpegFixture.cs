using System.Diagnostics;

namespace FFmpeg.Rx.IntegrationTests.Internal;

public sealed class FfmpegFixture : IAsyncLifetime
{
    public bool IsAvailable { get; private set; }

    public ValueTask InitializeAsync()
    {
        IsAvailable = CheckFfmpeg();
        if (!IsAvailable)
        {
            Assert.Skip("ffmpeg not found on PATH — skipping integration tests");
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private static bool CheckFfmpeg()
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
}
