namespace FFmpeg.Rx.IntegrationTests.Internal;

internal static class TestPaths
{
    internal static string TestData { get; } = ResolveTestData();

    private static string ResolveTestData()
    {
        var dir = AppContext.BaseDirectory;
        while (dir is not null)
        {
            var candidate = Path.Combine(dir, "testdata");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            dir = Path.GetDirectoryName(dir);
        }

        throw new DirectoryNotFoundException("Could not find testdata/ directory");
    }

    internal static string CreateTempOutputDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"ffmpeg_rx_it_{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        return dir;
    }
}
