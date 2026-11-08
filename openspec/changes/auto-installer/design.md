## Context

FFMpegCore has `FFMpegCore.Extensions.Downloader` as a separate NuGet package using the ffbinaries API. We want the same capability built into the core library, using more current binary sources.

## Decisions

### D1: Multi-source download registry

```csharp
internal static class BinarySources
{
    internal static string GetDownloadUrl(OSPlatform platform, Architecture arch) => (platform, arch) switch
    {
        _ when platform == OSPlatform.Windows && arch == Architecture.X64
            => "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip",
        _ when platform == OSPlatform.Linux && arch == Architecture.X64
            => "https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz",
        _ when platform == OSPlatform.Linux && arch == Architecture.Arm64
            => "https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-arm64-static.tar.xz",
        _ when platform == OSPlatform.OSX && arch == Architecture.X64
            => "https://evermeet.cx/ffmpeg/getrelease/zip",
        _ when platform == OSPlatform.OSX && arch == Architecture.Arm64
            => "https://evermeet.cx/ffmpeg/getrelease/zip",
        _ => throw new PlatformNotSupportedException($"No ffmpeg binary source for {platform}/{arch}")
    };
}
```

Not ffbinaries API - direct known URLs. Simpler, no JSON parsing, no API dependency.

**Trade-off:** URLs can change. Mitigation: well-established sources that have been stable for years. If a URL breaks, a library update fixes it.

### D2: Cache directory

Default: `~/.ffmpeg-rx/bin/` (`Path.Combine(Environment.GetFolderPath(SpecialFolder.UserProfile), ".ffmpeg-rx", "bin")`)

Configurable via `FfmpegOptions.Configure(o => o.DownloadDirectory = "/custom/path")`.

### D3: Extract logic per format

- `.zip` (Windows, macOS): `System.IO.Compression.ZipFile`
- `.tar.xz` (Linux): `TarReader` (.NET 10 has `System.Formats.Tar`)

Look for `ffmpeg`/`ffmpeg.exe` and `ffprobe`/`ffprobe.exe` in the archive, extract to cache dir, set executable permission on Linux/macOS.

### D4: API surface

```csharp
public static class Ffmpeg
{
    // Existing
    public static PipelineBuilder Input(string path) { ... }

    // New: ensure ffmpeg is available, download if missing
    public static async Task EnsureAsync(CancellationToken ct = default)
    {
        if (IsAvailable()) return;
        await DownloadAsync(ct: ct);
    }

    // New: explicitly download to specific or default directory
    public static async Task DownloadAsync(
        string? targetDir = null, CancellationToken ct = default) { ... }

    // New: check if ffmpeg is on PATH or configured
    public static bool IsAvailable() { ... }
}
```

### D5: Wire into FfmpegOptions

After download, set `FfmpegOptions.Global.BinaryPath` to the extracted binary. All subsequent sessions use it automatically.

### D6: No auto-download on first use

Explicit opt-in only. `FfmpegSession.Create()` does NOT call `EnsureAsync()`. The consumer decides when and whether to download. This avoids surprise network calls.

## Risks

- **Network dependency** - download fails if offline. Mitigation: clear error message, binary is cached for subsequent runs.
- **URL stability** - sources may change URLs. Mitigation: well-known stable sources, library update as fix.
- **Archive format** - Linux uses .tar.xz which needs `System.Formats.Tar` (available since .NET 7). Verify it works on .NET 10.
- **Permissions** - Linux/macOS need `chmod +x`. Use `File.SetUnixFileMode` (.NET 7+).
- **Binary trust** - downloading executables from the internet. Mitigation: well-known sources (gyan.dev is the official Windows build recommended by ffmpeg.org).
