using FFmpeg.Rx.IntegrationTests.Internal;

namespace FFmpeg.Rx.IntegrationTests.Probing;

public sealed class ProbeAudioTests : FfmpegTestBase
{
    [Fact(Timeout = 30000)]
    public async Task Probe_audio_only()
    {
        var info = await Ffprobe.AnalyseAsync(TestAsset("audio_only_3s.m4a"), TestContext.Current.CancellationToken);

        Assert.NotNull(info);
        Assert.Empty(info.VideoStreams);
        Assert.Single(info.AudioStreams);
        Assert.Contains("aac", info.AudioStreams[0].Codec, StringComparison.OrdinalIgnoreCase);
        Assert.InRange(info.Duration.TotalSeconds, 2.5, 3.5);
    }
}
