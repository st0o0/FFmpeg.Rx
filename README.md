# FFmpeg.Rx

[![CI](https://github.com/st0o0/FFmpeg.Rx/actions/workflows/ci.yml/badge.svg)](https://github.com/st0o0/FFmpeg.Rx/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/FFmpeg.Rx.svg)](https://www.nuget.org/packages/FFmpeg.Rx)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Reactive FFmpeg process wrapper using [R3](https://github.com/Cysharp/R3) Observables. Type-safe, composable, testable.

## Installation

```bash
dotnet add package FFmpeg.Rx
```

Requires `ffmpeg` on PATH (or provide a custom binary path via `Ffmpeg.At(path)`).

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

## Key Concepts

- **Fluent Builder API** - `Ffmpeg.Input().Output()` with scoped builders for each concern
- **Value Objects** - `VideoCodec`, `AudioCodec`, `Bitrate`, `Framerate`, `Preset` as `readonly record struct`
- **Type-Safe Encoding** - `StreamCopy` vs `VideoEncode` vs `AudioEncode` enforced at compile time
- **IFfmpegSession** - Interface for consumer mocking
- **R3 Observables** - All event streams use [R3](https://github.com/Cysharp/R3)

## Development

```bash
dotnet build src/FFmpeg.Rx.slnx
dotnet format src/FFmpeg.Rx.slnx --verify-no-changes
dotnet run --project src/FFmpeg.Rx.Tests/FFmpeg.Rx.Tests.csproj
dotnet run --project src/FFmpeg.Rx.IntegrationTests/FFmpeg.Rx.IntegrationTests.csproj
```

## License

[MIT](LICENSE)
