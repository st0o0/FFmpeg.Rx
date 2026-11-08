using System.Runtime.CompilerServices;

namespace FFmpeg.Rx.Tests;

public static class ModuleInit
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyDiffPlex.Initialize();
        UseProjectRelativeDirectory("verify");
        VerifierSettings.UniqueForRuntime();
        VerifierSettings.InitializePlugins();
    }
}
