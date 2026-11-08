namespace FFmpeg.Rx;

public sealed record FfmpegOptions
{
    public string? BinaryPath { get; init; }
    public string? FfprobeBinaryPath { get; init; }
    public string? TempDirectory { get; init; }
    public string? WorkingDirectory { get; init; }

    public static FfmpegOptions? Global { get; private set; }

    public static void Configure(FfmpegOptions options) => Global = options;

    public static void Reset() => Global = null;

    internal static string ResolveFfmpegBinary(string? perSessionOverride)
    {
        return perSessionOverride
            ?? Global?.BinaryPath
            ?? FindOnPath("ffmpeg")
            ?? "ffmpeg";
    }

    internal static string ResolveFfprobeBinary(string? perSessionOverride)
    {
        return perSessionOverride
            ?? Global?.FfprobeBinaryPath
            ?? FindOnPath("ffprobe")
            ?? "ffprobe";
    }

    private static string? FindOnPath(string name)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (pathEnv is null)
        {
            return null;
        }

        var separator = OperatingSystem.IsWindows() ? ';' : ':';
        string[] extensions = OperatingSystem.IsWindows() ? [".exe", ""] : [""];

        foreach (var dir in pathEnv.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var ext in extensions)
            {
                var fullPath = Path.Combine(dir, name + ext);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
        }

        string[] commonLocations = OperatingSystem.IsWindows()
            ? [@"C:\ProgramData\chocolatey\lib\ffmpeg\tools\ffmpeg\bin",
               @"C:\Program Files\ffmpeg\bin"]
            : ["/usr/bin", "/usr/local/bin", "/opt/homebrew/bin"];

        foreach (var dir in commonLocations)
        {
            foreach (var ext in extensions)
            {
                var fullPath = Path.Combine(dir, name + ext);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
        }

        return null;
    }
}
