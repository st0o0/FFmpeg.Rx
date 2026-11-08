## Why

Consumers need ffmpeg installed to use FFmpeg.Rx. On developer machines this is a manual step (winget, apt, brew). On CI it requires a setup step. An auto-download feature in the core library eliminates this friction with a single `await Ffmpeg.EnsureAsync()`.

## What Changes

### Auto-download with multi-source support

`Ffmpeg.EnsureAsync()` checks if ffmpeg is available. If not, downloads the correct binary for the current platform from a trusted source, extracts it, and configures `FfmpegOptions` to use it.

Sources by platform:
- **Windows**: gyan.dev (ffmpeg-release-essentials.zip)
- **Linux**: johnvansickle.com (ffmpeg-release-amd64-static.tar.xz)
- **macOS**: evermeet.cx (ffmpeg.zip)

### Caching

Downloaded binaries are cached in `~/.ffmpeg-rx/` (or a configurable path). Subsequent calls to `EnsureAsync()` return immediately if the binary exists and is executable.

### Explicit download API

`Ffmpeg.DownloadAsync(string? targetDir, CancellationToken)` for consumers who want explicit control over where binaries go.

### Platform detection

Uses `RuntimeInformation.OSArchitecture` and `RuntimeInformation.IsOSPlatform` for reliable detection. No ffbinaries dependency - direct downloads from well-known sources.

### In Core-Lib, opt-in

No new NuGet packages. Uses `HttpClient` and `System.IO.Compression` (both in .NET BCL). The feature is opt-in - consumers must call `EnsureAsync()` or `DownloadAsync()` explicitly. No automatic download on first use.
