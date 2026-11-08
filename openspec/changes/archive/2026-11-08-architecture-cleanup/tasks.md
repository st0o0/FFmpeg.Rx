## 1. Add CliWrap dependency

- [x] 1.1 Add `CliWrap` to `Directory.Packages.props`
- [x] 1.2 Add `<PackageReference Include="CliWrap" />` to `FFmpeg.Rx.csproj`
- [x] 1.3 Verify build compiles with CliWrap

## 2. Rewrite FfmpegSession on CliWrap

- [x] 2.1 Remove `FfmpegSession(FfmpegPipeline)` constructor — keep only `PipelineConfig` path
- [x] 2.2 Remove dummy FfmpegPipeline creation hack in PipelineConfig constructor
- [x] 2.3 Implement `RunAsync` using `Cli.Wrap().ListenAsync()` — stdout to ProgressParser, stderr to LogEvent/ErrorClassifier
- [x] 2.4 Implement graceful cancellation: write `q` to stdin pipe when no pipe input, CliWrap graceful token as fallback
- [x] 2.5 Implement pipe input: if PipeInputConfig has Stream, use `PipeSource.FromStream` + `-i pipe:0`
- [x] 2.6 Implement pipe output: if PipeDestination has Stream, use `PipeTarget.ToStream` + `pipe:1`
- [x] 2.7 Remove `FfmpegSession.Create(FfmpegPipeline)` — only `Create(PipelineConfig)` remains

## 3. Rewrite ProbeRunner on CliWrap

- [x] 3.1 Replace manual Process.Start with `Cli.Wrap(ffprobe).ExecuteBufferedAsync()`
- [x] 3.2 Simplify error handling — CliWrap captures stderr automatically

## 4. Delete old model

- [x] 4.1 Delete `Models/FfmpegPipeline.cs` (FfmpegPipeline, FfmpegInput, UrlInput, FileInput, TeeOutput, TeeTarget)
- [x] 4.2 Delete `Models/FfmpegOutput.cs` (FfmpegOutput, StreamMapping)
- [x] 4.3 Delete `Models/PipeInput.cs`
- [x] 4.4 Delete `Models/PipeOutput.cs`
- [x] 4.5 Delete `Internal/ProcessRunner.cs`
- [x] 4.6 Delete `Internal/ArgumentBuilder.cs`
- [x] 4.7 Delete `Builder/OutputOptions.cs`
- [x] 4.8 Delete `Builder/InputOptions.cs`
- [x] 4.9 Delete `IO/` directory (IPipeSource, IPipeSink, StreamPipeSource, StreamPipeSink)
- [x] 4.10 Fix all compilation errors from deleted types

## 5. Rewire PipelineBuilder

- [x] 5.1 Rewrite `PipelineBuilder` to collect `InputConfig` and produce `PipelineConfig` via `OutputBuilder`
- [x] 5.2 Rewrite `Ffmpeg` static entry to create `FileInputConfig`/`UrlInputConfig`
- [x] 5.3 Add `Ffmpeg.Input(Stream, string format)` overload for pipe input
- [x] 5.4 Rewrite `PipelineBuilder.Output()` to use `OutputBuilder` → `OutputConfig` → `PipelineConfig` → `FfmpegSession.Create`
- [x] 5.5 Rewrite `PipelineBuilder.OutputPipe()` for pipe output via `PipeDestination`
- [x] 5.6 Rewrite `PipelineBuilder.OutputNull()` using `NullDestination`
- [x] 5.7 Rewrite `MultiOutputBuilder` to collect `OutputConfig` and produce `PipelineConfig`
- [x] 5.8 Rewrite `PipelineBuilder.OutputTee()` using `OutputConfig.Tee`
- [x] 5.9 Delete `TeeBuilder` if TeeOutput can be built directly, or keep and adapt

## 6. Update tests

- [x] 6.1 Delete old-model test references in `ArgumentBuilderTests` (or delete the file if all tests are in `ArgumentSerializerTests`)
- [x] 6.2 Update `FfmpegSessionIntegrationTests` to use only `PipelineConfig` path
- [x] 6.3 Update `PipelineBuilderTests` to test new builder → PipelineConfig flow
- [x] 6.4 Fix any remaining references to deleted types across all test files
- [x] 6.5 Verify all 186 unit tests pass
- [x] 6.6 Verify all 21 integration tests pass

## 7. Finalize

- [x] 7.1 `dotnet build src/FFmpeg.Rx.slnx` — zero errors, zero warnings
- [x] 7.2 `dotnet format src/FFmpeg.Rx.slnx --verify-no-changes` — clean
- [x] 7.3 Update AGENTS.md — remove IO/ and ProcessRunner from structure, add CliWrap to dependencies
- [x] 7.4 Update CLAUDE.md if needed
- [x] 7.5 Verify no dead code / unused usings remain
