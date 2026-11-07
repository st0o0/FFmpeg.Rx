using System.Diagnostics;
using FFmpeg.Rx.Probing;

namespace FFmpeg.Rx.Internal;

internal static class ProbeRunner
{
    internal static async Task<MediaInfo> RunAsync(string arguments, string binaryPath, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = binaryPath,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process process;
        try
        {
            process = Process.Start(psi)
                ?? throw new InvalidOperationException($"Failed to start process: {binaryPath}");
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            throw new FfprobeException(1, $"Failed to start ffprobe: {ex.Message}");
        }

        using (process)
        {
            var stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
            var stderrTask = process.StandardError.ReadToEndAsync(ct);

            await process.WaitForExitAsync(ct).ConfigureAwait(false);

            var stdout = await stdoutTask.ConfigureAwait(false);
            var stderr = await stderrTask.ConfigureAwait(false);

            if (process.ExitCode != 0)
            {
                throw new FfprobeException(process.ExitCode, stderr.Trim());
            }

            return ProbeResultParser.Parse(stdout);
        }
    }
}
