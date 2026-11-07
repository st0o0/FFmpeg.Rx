using FFmpeg.Rx.Internal;
using FFmpeg.Rx.Probing;

namespace FFmpeg.Rx;

public static class Ffprobe
{
    public static Task<MediaInfo> AnalyseAsync(string path, CancellationToken ct = default)
    {
        return AnalyseAsync(path, null, ct);
    }

    public static Task<MediaInfo> AnalyseAsync(string path, FfprobeOptions? options, CancellationToken ct = default)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Input file not found: {path}", path);
        }

        var binary = FfmpegOptions.ResolveFfprobeBinary(options?.BinaryPath);
        var arguments = $"-print_format json -show_format -show_streams -show_chapters \"{path}\"";
        return ProbeRunner.RunAsync(arguments, binary, ct);
    }

    public static Task<MediaInfo> AnalyseAsync(Uri url, CancellationToken ct = default)
    {
        return AnalyseAsync(url, null, ct);
    }

    public static Task<MediaInfo> AnalyseAsync(Uri url, FfprobeOptions? options, CancellationToken ct = default)
    {
        var binary = FfmpegOptions.ResolveFfprobeBinary(options?.BinaryPath);
        var arguments = $"-print_format json -show_format -show_streams -show_chapters \"{url}\"";
        return ProbeRunner.RunAsync(arguments, binary, ct);
    }
}
