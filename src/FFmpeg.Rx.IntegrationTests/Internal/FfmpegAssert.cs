namespace FFmpeg.Rx.IntegrationTests.Internal;

internal static class FfmpegAssert
{
    internal static void OutputExists(string path)
    {
        Assert.True(File.Exists(path), $"Output file does not exist: {path}");
        Assert.True(new FileInfo(path).Length > 0, $"Output file is empty: {path}");
    }

    internal static async Task HasVideoStream(string path, string expectedCodec)
    {
        var info = await Ffprobe.AnalyseAsync(path);
        Assert.NotNull(info);
        Assert.Contains(info.VideoStreams, v =>
            v.Codec.Contains(expectedCodec, StringComparison.OrdinalIgnoreCase));
    }

    internal static async Task HasAudioStream(string path, string expectedCodec)
    {
        var info = await Ffprobe.AnalyseAsync(path);
        Assert.NotNull(info);
        Assert.Contains(info.AudioStreams, a =>
            a.Codec.Contains(expectedCodec, StringComparison.OrdinalIgnoreCase));
    }

    internal static async Task HasResolution(string path, int width, int height)
    {
        var info = await Ffprobe.AnalyseAsync(path);
        Assert.NotNull(info);
        var video = info.VideoStreams.First();
        Assert.Equal(width, video.Width);
        Assert.Equal(height, video.Height);
    }

    internal static async Task HasDurationApprox(string path, TimeSpan expected, double toleranceSec = 0.5)
    {
        var info = await Ffprobe.AnalyseAsync(path);
        Assert.NotNull(info);
        var diff = Math.Abs(info.Duration.TotalSeconds - expected.TotalSeconds);
        Assert.True(diff <= toleranceSec,
            $"Duration {info.Duration.TotalSeconds:F2}s differs from expected {expected.TotalSeconds:F2}s by {diff:F2}s (tolerance: {toleranceSec}s)");
    }

    internal static async Task HasAudioStreamCount(string path, int count)
    {
        var info = await Ffprobe.AnalyseAsync(path);
        Assert.NotNull(info);
        Assert.Equal(count, info.AudioStreams.Count);
    }

    internal static async Task HasSubtitleStream(string path)
    {
        var info = await Ffprobe.AnalyseAsync(path);
        Assert.NotNull(info);
        Assert.NotEmpty(info.SubtitleStreams);
    }
}
