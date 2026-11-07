namespace FFmpeg.Rx.Probing;

public sealed class FfprobeException : Exception
{
    public int ExitCode { get; }
    public string StandardError { get; }

    public FfprobeException(int exitCode, string standardError)
        : base($"ffprobe exited with code {exitCode}: {standardError}")
    {
        ExitCode = exitCode;
        StandardError = standardError;
    }
}
