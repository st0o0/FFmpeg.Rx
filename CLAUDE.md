# FFmpeg.Rx

Reactive FFmpeg process wrapper using R3 Observables.

## Build & test

```powershell
dotnet build src/FFmpeg.Rx.slnx
dotnet format src/FFmpeg.Rx.slnx --verify-no-changes
dotnet run --project src/FFmpeg.Rx.Tests/FFmpeg.Rx.Tests.csproj
```

Tests use xUnit v3 on Microsoft.Testing.Platform — `dotnet run`, not `dotnet test`.

## Architecture

```
Ffmpeg (static entry)
  → PipelineBuilder (fluent API)
    → FfmpegPipeline (immutable records)
      → FfmpegSession (one process, Observable lifecycle)
        → ProcessRunner (internal, Process.Start + R3 Subjects)
```

- **Event hierarchy**: `FfmpegEvent` (abstract) → `StartedEvent`, `ProgressEvent`,
  `LogEvent`, `CompletedEvent`, `FailedEvent`. Single `Observable<FfmpegEvent>` stream
  with typed convenience projections (`.Progress`, `.Log`).
- **Pipeline model**: Immutable records (`FfmpegPipeline`, `FfmpegInput`, `FfmpegOutput`,
  `StreamMapping`). Builder is syntactic sugar — records are the testable truth.
- **One session = one process**: Build → Run → Done. No reuse.
- **Pure-function internals**: `ProgressParser`, `ErrorClassifier`, `ArgumentBuilder`
  are static, side-effect-free, directly testable.

## C# conventions

- Allman braces, 4-space indent, `sealed` by default
- `var` when type is apparent, `_field` for private fields
- No XML docs, no `this.` qualifier
- Enums start with `Unknown = 0`
- `dotnet format` enforced

## Dependencies

- R3 (Cysharp) — reactive Observable implementation
- xUnit v3 — testing
