using CliWrap;
using CliWrap.Buffered;
using FFmpeg.Rx.Probing;

namespace FFmpeg.Rx.Internal;

internal static class ProbeRunner
{
    internal static async Task<MediaInfo> RunAsync(string arguments, string binaryPath, CancellationToken ct)
    {
        try
        {
            var result = await Cli.Wrap(binaryPath)
                .WithArguments(arguments)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(ct)
                .ConfigureAwait(false);

            if (result.ExitCode != 0)
            {
                throw new FfprobeException(result.ExitCode, result.StandardError.Trim());
            }

            return ProbeResultParser.Parse(result.StandardOutput);
        }
        catch (Exception ex) when (ex is not FfprobeException and not OperationCanceledException)
        {
            throw new FfprobeException(1, $"Failed to start ffprobe: {ex.Message}");
        }
    }
}
