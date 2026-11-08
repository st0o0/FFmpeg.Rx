## 1. Platform detection

- [ ] 1.1 Create `Internal/PlatformDetector.cs` with `DetectPlatform()` returning (OSPlatform, Architecture)
- [ ] 1.2 Create `Internal/BinarySources.cs` with `GetDownloadUrl(OSPlatform, Architecture)` returning URL
- [ ] 1.3 Unit tests for platform-to-URL mapping

## 2. Download and extract

- [ ] 2.1 Create `Internal/FfmpegDownloader.cs` with `DownloadAsync(string targetDir, CancellationToken)`
- [ ] 2.2 Implement HTTP download with progress (HttpClient + Stream)
- [ ] 2.3 Implement zip extraction (Windows, macOS) — find ffmpeg/ffprobe in archive
- [ ] 2.4 Implement tar.xz extraction (Linux) — System.Formats.Tar + XZ decompression
- [ ] 2.5 Set executable permission on Linux/macOS (`File.SetUnixFileMode`)
- [ ] 2.6 Unit tests for archive entry name matching logic

## 3. Cache management

- [ ] 3.1 Default cache directory: `~/.ffmpeg-rx/bin/`
- [ ] 3.2 Add `DownloadDirectory` property to `FfmpegOptions`
- [ ] 3.3 Check if cached binary exists and is executable before downloading
- [ ] 3.4 Unit test for cache path resolution

## 4. Public API

- [ ] 4.1 Add `Ffmpeg.IsAvailable()` — try to run `ffmpeg -version`, return bool
- [ ] 4.2 Add `Ffmpeg.DownloadAsync(string? targetDir, CancellationToken)` — download + extract + set binary path
- [ ] 4.3 Add `Ffmpeg.EnsureAsync(CancellationToken)` — IsAvailable ? return : DownloadAsync
- [ ] 4.4 Wire downloaded path into `FfmpegOptions.Global.BinaryPath`
- [ ] 4.5 Unit test for IsAvailable (mock-friendly via binary path)
- [ ] 4.6 Integration test: EnsureAsync on machine with ffmpeg returns immediately

## 5. Finalize

- [ ] 5.1 All unit tests pass
- [ ] 5.2 All integration tests pass
- [ ] 5.3 `dotnet build` zero errors, zero warnings
- [ ] 5.4 `dotnet format --verify-no-changes` clean
- [ ] 5.5 Update README with EnsureAsync usage example
- [ ] 5.6 Update AGENTS.md with downloader in architecture
