using PublicApiGenerator;

namespace FFmpeg.Rx.Tests;

public sealed class PublicApiTests
{
    private static readonly ApiGeneratorOptions _apiOptions = new()
    {
        ExcludeAttributes =
        [
            "System.Reflection.AssemblyMetadataAttribute",
            "System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute",
            "System.Runtime.CompilerServices.AsyncStateMachineAttribute",
            "System.Runtime.CompilerServices.IteratorStateMachineAttribute"
        ]
    };

    [Fact]
    public Task PublicApi_MatchesApprovedSnapshot()
    {
        return Verify(typeof(Ffmpeg).Assembly.GeneratePublicApi(_apiOptions));
    }
}
