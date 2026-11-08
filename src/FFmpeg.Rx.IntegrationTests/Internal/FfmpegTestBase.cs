namespace FFmpeg.Rx.IntegrationTests.Internal;

public abstract class FfmpegTestBase : IAsyncLifetime, IClassFixture<FfmpegFixture>
{
    private string _outputDir = null!;

    public ValueTask InitializeAsync()
    {
        _outputDir = TestPaths.CreateTempOutputDir();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        if (Directory.Exists(_outputDir))
        {
            Directory.Delete(_outputDir, true);
        }

        return ValueTask.CompletedTask;
    }

    protected string TestAsset(string name)
    {
        return Path.Combine(TestPaths.TestData, name);
    }

    protected string OutputPath(string name)
    {
        return Path.Combine(_outputDir, name);
    }
}
