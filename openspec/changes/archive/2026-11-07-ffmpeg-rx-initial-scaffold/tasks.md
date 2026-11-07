## 1. Project scaffold

- [x] 1.1 Rename `src/ProjectName/` → `src/FFmpeg.Rx/`, `src/ProjectName.Tests/` → `src/FFmpeg.Rx.Tests/`, `src/ProjectName.slnx` → `src/FFmpeg.Rx.slnx`. Update csproj filenames, solution references, and `Directory.Build.props` URLs.
- [x] 1.2 Replace Terp dependency with R3 in `Directory.Packages.props`. Remove `Terp` PackageReference from library csproj, add `R3`.
- [x] 1.3 Update `release-please-config.json` and `.release-please-manifest.json` if they reference ProjectName.
- [x] 1.4 Delete placeholder `Class1.cs` and `UnitTest1.cs`.
- [x] 1.5 Verify `dotnet build` and `dotnet run --project src/FFmpeg.Rx.Tests/` both succeed (empty project, zero tests).

## 2. Event hierarchy and models

- [x] 2.1 Create `Events/FfmpegEvent.cs`: `abstract record FfmpegEvent` and all five sealed subtypes (`StartedEvent`, `ProgressEvent`, `LogEvent`, `CompletedEvent`, `FailedEvent`).
- [x] 2.2 Create `Models/FfmpegPipeline.cs`: `FfmpegPipeline`, `FfmpegInput` (abstract), `UrlInput`, `FileInput` records.
- [x] 2.3 Create `Models/FfmpegOutput.cs`: `FfmpegOutput`, `StreamMapping` records.
- [x] 2.4 Create `Models/FfmpegResult.cs`: result record with `Success`, `ExitCode`, `Error`, `Duration`, `FailureKind`.
- [x] 2.5 Create `Models/Enums.cs`: `StreamType`, `FailureKind`, `LogLevel` enums (all with `Unknown = 0`).
- [x] 2.6 Create `Models/Codec.cs`: static class with `Copy = "copy"` constant.

## 3. Pure-function internals

- [x] 3.1 Create `Internal/ProgressParser.cs`: stateful line accumulator, pure block-to-event conversion. Port from FunkArr's `ParseProgressLine`.
- [x] 3.2 Create `Internal/ErrorClassifier.cs`: `Classify(string?)` → `FailureKind` and `ExtractError(string?)` → `string`. Port from FunkArr's `ClassifyFailure` and `ExtractError`.
- [x] 3.3 Create `Internal/ArgumentBuilder.cs`: converts `FfmpegPipeline` record to a CLI argument string. Handles input options (user-agent, proxy), stream mappings (`-map`, `-c:v`), output args, metadata, and `-progress pipe:1`.
- [x] 3.4 Tests for `ProgressParser`: complete block, partial block, speed N/A, missing fields, multiple blocks, malformed lines.
- [x] 3.5 Tests for `ErrorClassifier`: all transient patterns, permanent patterns, unknown, null/empty, `ExtractError` line selection.
- [x] 3.6 Tests for `ArgumentBuilder`: URL input with proxy/UA, file input, stream mappings, custom args, metadata, multi-input, progress flag.

## 4. Process runner

- [x] 4.1 Create `Internal/ProcessRunner.cs`: wraps `System.Diagnostics.Process`, starts FFmpeg, reads stdout line-by-line (feeding `ProgressParser`), collects stderr, reports exit code. Pushes events into an R3 `Subject<FfmpegEvent>`.
- [x] 4.2 Handle cancellation: kill process on token cancellation, emit `FailedEvent(-1, "Cancelled")`.
- [x] 4.3 Handle binary-not-found: descriptive error when FFmpeg executable is missing.

## 5. FfmpegSession

- [x] 5.1 Create `FfmpegSession.cs`: holds `Observable<FfmpegEvent> Events`, typed projections (`.Progress`, `.Log`), `RunAsync(CancellationToken)` → `Task<FfmpegResult>`, `IAsyncDisposable`.
- [x] 5.2 Enforce single-use: `InvalidOperationException` on second `RunAsync` call.
- [x] 5.3 Static factory `FfmpegSession.Create(FfmpegPipeline, string? binaryPath)` for direct construction without builder.
- [x] 5.4 Tests for session lifecycle: subscribe before run, run without subscribers, double run, dispose during run, cancellation.

## 6. Fluent builder

- [x] 6.1 Create `Ffmpeg.cs`: static entry point with `Input(Uri, Action<InputOptions>?)`, `Input(string)`, `At(string)`.
- [x] 6.2 Create `Builder/PipelineBuilder.cs`: chainable `.Input()`, `.Output()`, `.WithProgressReporting()`, `.Build()`.
- [x] 6.3 Create `Builder/InputOptions.cs`: `.UserAgent(string)`, `.Proxy(string)`, `.Arg(string)`.
- [x] 6.4 Create `Builder/OutputOptions.cs`: `.Map(int, int, StreamType, string)`, `.Arg(string)`, `.Metadata(string, string)`.
- [x] 6.5 Tests for builder: full pipeline construction, input options, output mappings, progress flag, custom binary path, builder produces correct pipeline records.

## 7. Integration and verification

- [x] 7.1 End-to-end test: build pipeline → create session → subscribe to events → run with a real `ffmpeg -version` (no media, just verifying process lifecycle works).
- [x] 7.2 `dotnet build` with 0 warnings, `dotnet format --verify-no-changes`.
- [x] 7.3 Verify all tests pass: `dotnet run --project src/FFmpeg.Rx.Tests/`.
- [x] 7.4 Create `CLAUDE.md` with project conventions, build commands, and architecture overview.
