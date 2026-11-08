# FFmpeg.Rx

[![NuGet](https://img.shields.io/nuget/v/FFmpeg.Rx.svg?style=flat-square)](https://www.nuget.org/packages/FFmpeg.Rx)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FFmpeg.Rx.svg?style=flat-square)](https://www.nuget.org/packages/FFmpeg.Rx/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)
[![Dotnet](https://img.shields.io/badge/dotnet-10.0-5027d5?style=flat-square)](https://dotnet.microsoft.com)

Reactive FFmpeg process wrapper using [R3](https://github.com/Cysharp/R3) Observables. Type-safe, composable, testable.

---

## Why FFmpeg.Rx?

Most .NET FFmpeg wrappers pass encoding options as raw strings — codec names, bitrate values, filter expressions. Typos compile fine and fail at runtime. FFmpeg.Rx replaces string bags with typed records: every codec, bitrate, preset, pixel format, and container is a .NET type with static factories and a `Custom()` escape hatch. The pipeline is a record tree you can inspect, test, and serialize — builders are syntactic sugar on top.

Events (progress, log, lifecycle) come as R3 Observables, not callbacks. One session wraps one process. Build → Run → Done.

---

## Features

### Pipeline

- **Type-safe codecs** — `VideoCodec.H264`, `AudioCodec.Aac`, `SubtitleCodec.Srt` as `readonly record struct`
- **Disjoint sum types** — `StreamCopy | VideoEncode | AudioEncode | SubtitleEncode` enforced at compile time
- **Value objects** — `Bitrate`, `Framerate`, `Preset`, `PixelFormat`, `ContainerFormat` with `Custom()` escape hatch
- **Immutable records** — `PipelineSpec`, `OutputSpec`, `StreamSpec` as the testable truth

### Events

- **R3 Observables** — `Progress`, `Log`, typed `FfmpegEvent` hierarchy
- **Progress reporting** — speed, bytes written, time position, percentage via known duration

### Probing

- **ffprobe integration** — `Ffmpeg.Probe()` returns strongly-typed `MediaInfo`
- **Source-generated JSON** — AOT-compatible deserialization via `JsonSerializerContext`
- **Stream info** — video, audio, subtitle streams with codec, resolution, sample rate, chapters

### Convenience

- **Snapshots** — extract single frames with `Ffmpeg.Snapshot()`
- **Concatenation** — join files with `Ffmpeg.Concat()`
- **Custom binary path** — `Ffmpeg.At("/path/to/ffmpeg")`
- **Pipe I/O** — stdin/stdout streaming via CliWrap

### Extensibility

- **`CustomArg()` escape hatch** — on every builder for uncovered FFmpeg flags
- **`IFfmpegSession` interface** — mock sessions in consumer tests
- **Filters** — scale, crop, fade, pad, and custom filter expressions

---

## Installation

```bash
dotnet add package FFmpeg.Rx
```

Requires `ffmpeg` on PATH (or provide a custom binary path via `Ffmpeg.At(path)`).

**Requirements:** .NET 10.0 or higher

## Quick Start

```csharp
using FFmpeg.Rx;
using FFmpeg.Rx.Codecs;

// Transcode with H264 + AAC
await using var session = Ffmpeg.Input("input.mp4")
    .Output("output.mp4", o => o
        .Stream(0, 0, StreamType.Video, s => s
            .Encode(VideoCodec.H264, e => e
                .Quality(Quality.Crf(23))
                .Preset(Preset.Slow)))
        .Stream(0, 0, StreamType.Audio, s => s
            .Encode(AudioCodec.Aac, e => e
                .Bitrate(Bitrate.Kbps(192)))));

var result = await session.RunAsync();
```

## Stream Copy (Remux)

```csharp
await using var session = Ffmpeg.Input("input.mp4")
    .Output("output.mkv", o => o
        .Stream(0, 0, StreamType.Video, s => s.Copy())
        .Stream(0, 0, StreamType.Audio, s => s.Copy()));

await session.RunAsync();
```

## Progress Events

```csharp
await using var session = Ffmpeg.Input("input.mp4")
    .WithProgressReporting()
    .Output("output.mp4", o => o
        .Stream(0, 0, StreamType.Video, s => s
            .Encode(VideoCodec.H264, e => e.Quality(Quality.Crf(23)))));

session.Progress.Subscribe(p =>
    Console.WriteLine($"Speed: {p.Speed:F1}x, Written: {p.BytesWritten} bytes"));

session.Log.Subscribe(log =>
    Console.WriteLine($"[{log.Level}] {log.Line}"));

var result = await session.RunAsync();
```

## Filters

```csharp
await using var session = Ffmpeg.Input("input.mp4")
    .Output("scaled.mp4", o => o
        .Stream(0, 0, StreamType.Video, s => s
            .Encode(VideoCodec.H264, e => e.Quality(Quality.Crf(23))))
        .VideoFilter(f => f.Scale(1920, 1080)));

await session.RunAsync();
```

## Probing

```csharp
var info = await Ffmpeg.Probe("video.mp4");

Console.WriteLine($"Duration: {info.Duration}");
Console.WriteLine($"Video: {info.PrimaryVideo?.Codec} {info.PrimaryVideo?.Width}x{info.PrimaryVideo?.Height}");
Console.WriteLine($"Audio: {info.PrimaryAudio?.Codec} {info.PrimaryAudio?.SampleRate}Hz");
```

## Snapshots and Concat

```csharp
// Extract a single frame
await Ffmpeg.Snapshot("video.mp4", TimeSpan.FromSeconds(5), "thumb.jpg");

// Concatenate videos (same codec)
await Ffmpeg.Concat(["part1.mp4", "part2.mp4"], "full.mp4");
```

## Custom Binary Path

```csharp
await using var session = Ffmpeg.At("/usr/local/bin/ffmpeg")
    .Input("input.mp4")
    .Output("output.mp4", o => o
        .Stream(0, 0, StreamType.Video, s => s.Copy())
        .Stream(0, 0, StreamType.Audio, s => s.Copy()));
```

---

## Architecture

Three-layer design — each layer is independently testable:

```
Layer 1: Pipeline records     what the user wants    PipelineSpec, OutputSpec, CodecSpec...
                              (immutable, inspectable, serializable)
                                        |
Layer 2: Scoped builders      how the user says it   PipelineBuilder, OutputBuilder, StreamBuilder...
                              (fluent API sugar)
                                        |
Layer 3: Serializer + CliWrap how it gets done        ArgumentSerializer → CliWrap process execution
                              (pattern-match records → CLI args → run)
```

**Core types:**

| Type | Role |
|------|------|
| `PipelineSpec` | Root record: inputs + outputs + global options |
| `InputSpec` | Abstract: `FileInput`, `UrlInput`, `PipeInput` |
| `OutputSpec` | Destination, streams, container, filters, metadata |
| `CodecSpec` | `StreamCopy` \| `VideoEncode` \| `AudioEncode` \| `SubtitleEncode` |
| `IFfmpegSession` | Consumer-facing interface (mockable) |
| `FfmpegSession` | CliWrap execution, R3 event bridging |

---

## Building from Source

```bash
dotnet build src/FFmpeg.Rx.slnx
dotnet format src/FFmpeg.Rx.slnx --verify-no-changes
dotnet run --project src/FFmpeg.Rx.Tests/FFmpeg.Rx.Tests.csproj
dotnet run --project src/FFmpeg.Rx.IntegrationTests/FFmpeg.Rx.IntegrationTests.csproj
```

---

## Contributing

Contributions are welcome. Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for your changes
4. Ensure all tests pass and `dotnet format` is clean
5. Submit a Pull Request

---

## License

[MIT](LICENSE)
