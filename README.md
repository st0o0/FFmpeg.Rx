# FFmpeg.Rx

[![CI](https://github.com/st0o0/FFmpeg.Rx/actions/workflows/ci.yml/badge.svg)](https://github.com/st0o0/FFmpeg.Rx/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/FFmpeg.Rx.svg)](https://www.nuget.org/packages/FFmpeg.Rx)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Reactive FFmpeg process wrapper using [R3](https://github.com/Cysharp/R3) Observables. Type-safe, composable, testable.

## Installation

```bash
dotnet add package FFmpeg.Rx
```

Requires `ffmpeg` on PATH (or provide a custom binary path).

## Quick Start

```csharp
using FFmpeg.Rx;
using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

// Transcode a video
await using var session = FfmpegSession.Create(new PipelineSpec(
    [new FileInputSpec("input.mp4")],
    [new OutputSpec(
        new FileDestination("output.mp4"),
        Streams: [
            new StreamSpec(0, 0, StreamType.Video,
                new VideoEncode(VideoCodec.H264,
                    new VideoEncodingOptions(Quality: Quality.Crf(23), Preset: Preset.Slow))),
            new StreamSpec(0, 0, StreamType.Audio,
                new AudioEncode(AudioCodec.Aac,
                    new AudioEncodingOptions(Bitrate: Bitrate.Kbps(192))))
        ])]));

var result = await session.RunAsync();
Console.WriteLine(result.Success ? "Done!" : result.Error);
```

## Fluent Builder API

```csharp
// Stream copy (remux)
await using var session = Ffmpeg.Input("input.mp4")
    .Output("output.mkv", o => o
        .Stream(0, 0, StreamType.Video, s => s.Copy())
        .Stream(0, 0, StreamType.Audio, s => s.Copy()));

await session.RunAsync();
```

## Observable Events

```csharp
await using var session = Ffmpeg.Input("input.mp4")
    .WithProgressReporting()
    .Output("output.mp4", o => o
        .Stream(0, 0, StreamType.Video, s => s
            .Encode(VideoCodec.H264, e => e.Quality(Quality.Crf(23)))));

// Subscribe to progress events via R3
session.Progress.Subscribe(p =>
    Console.WriteLine($"Speed: {p.Speed:F1}x, Written: {p.BytesWritten} bytes"));

session.Log.Subscribe(log =>
    Console.WriteLine($"[{log.Level}] {log.Line}"));

var result = await session.RunAsync();
```

## Probing

```csharp
var info = await Ffprobe.AnalyseAsync("video.mp4");

Console.WriteLine($"Duration: {info.Duration}");
Console.WriteLine($"Video: {info.PrimaryVideo?.Codec} {info.PrimaryVideo?.Width}x{info.PrimaryVideo?.Height}");
Console.WriteLine($"Audio: {info.PrimaryAudio?.Codec} {info.PrimaryAudio?.SampleRate}Hz");
```

## Filters

```csharp
// Simple video filter
await using var session = FfmpegSession.Create(new PipelineSpec(
    [new FileInputSpec("input.mp4")],
    [new OutputSpec(
        new FileDestination("scaled.mp4"),
        Streams: [new StreamSpec(0, 0, StreamType.Video,
            new VideoEncode(VideoCodec.H264, new VideoEncodingOptions(Quality: Quality.Crf(23))))],
        Filters: new SimpleFilterSpec(
            VideoFilters: [new ScaleFilter(1920, 1080)]))]));
```

## Key Concepts

- **Value Objects** - `VideoCodec`, `AudioCodec`, `Bitrate`, `Framerate`, `Preset` etc. are `readonly record struct` with static factories and `Custom()` escape hatch
- **Disjoint Sum Types** - `CodecSpec` = `StreamCopy | VideoEncode | AudioEncode | SubtitleEncode`. CRF and Bitrate are mutually exclusive via `QualitySpec`
- **Three Layers** - Pipeline records (domain), Builders (fluent API), CliWrap execution
- **IFfmpegSession** - Interface for consumer mocking. Program against the interface, not the implementation
- **R3 Observables** - All event streams use [R3](https://github.com/Cysharp/R3), the modern reactive implementation

## Development

```bash
dotnet build src/FFmpeg.Rx.slnx
dotnet format src/FFmpeg.Rx.slnx --verify-no-changes
dotnet run --project src/FFmpeg.Rx.Tests/FFmpeg.Rx.Tests.csproj           # unit tests
dotnet run --project src/FFmpeg.Rx.IntegrationTests/FFmpeg.Rx.IntegrationTests.csproj  # requires ffmpeg
```

## License

[MIT](LICENSE)
