using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Models;
using FFmpeg.Rx.Probing;

namespace FFmpeg.Rx;

public static class Ffmpeg
{
    public static PipelineBuilder Input(Uri url) => new PipelineBuilder(null).AddInput(new UrlInputSpec(url));

    public static PipelineBuilder Input(string filePath) =>
        new PipelineBuilder(null).AddInput(new FileInputSpec(filePath));

    public static PipelineBuilder Input(Stream source, string format) =>
        new PipelineBuilder(null).AddInput(new PipeInputSpec(source, format));

    public static PipelineBuilder At(string binaryPath) => new(binaryPath);

    public static Task<MediaInfo> Probe(string path, CancellationToken ct = default)
    {
        return Ffprobe.AnalyseAsync(path, ct);
    }

    public static async Task<FfmpegResult> Snapshot(string inputPath, TimeSpan time, string outputPath, CancellationToken ct = default)
    {
        var spec = new PipelineSpec(
            [new FileInputSpec(inputPath) { Options = new InputOptionSpec(Seek: time) }],
            [new OutputSpec(
                new FileDestination(outputPath),
                CustomArgs: ["-frames:v 1", "-update 1"])]);
        var session = FfmpegSession.Create(spec);
        await using (session.ConfigureAwait(false))
        {
            return await session.RunAsync(ct).ConfigureAwait(false);
        }
    }

    public static async Task<FfmpegResult> Concat(IEnumerable<string> inputPaths, string outputPath, CancellationToken ct = default)
    {
        var listFile = Path.Combine(Path.GetTempPath(), $"ffmpeg_rx_concat_{Guid.NewGuid():N}.txt");
        try
        {
            var lines = inputPaths.Select(p => $"file '{p.Replace("'", "'\\''")}'");
            await File.WriteAllLinesAsync(listFile, lines, ct).ConfigureAwait(false);

            var spec = new PipelineSpec(
                [new FileInputSpec(listFile) { Options = new InputOptionSpec(CustomArgs: ["-f concat", "-safe 0"]) }],
                [new OutputSpec(
                    new FileDestination(outputPath),
                    CustomArgs: ["-c copy"])]);
            var session = FfmpegSession.Create(spec);
            await using (session.ConfigureAwait(false))
            {
                return await session.RunAsync(ct).ConfigureAwait(false);
            }
        }
        finally
        {
            if (File.Exists(listFile))
            {
                File.Delete(listFile);
            }
        }
    }
}
