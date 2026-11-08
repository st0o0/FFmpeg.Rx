# Release Notes

## v1.0.0

Initial release.

### Features

- Typed domain model: `PipelineSpec`, `OutputSpec`, `StreamSpec`, `CodecSpec` hierarchy
- Value objects: `VideoCodec`, `AudioCodec`, `Bitrate`, `Framerate`, `Preset`, `VideoProfile`, `Tune`, `PixelFormat`, `ContainerFormat`, `HWAccel`
- Disjoint sum types: `CodecSpec` (StreamCopy/VideoEncode/AudioEncode/SubtitleEncode), `QualitySpec` (CRF/Bitrate)
- Scoped fluent builders: `PipelineBuilder`, `OutputBuilder`, `StreamBuilder`, `VideoEncodingBuilder`, `AudioEncodingBuilder`
- R3 Observable event stream: `StartedEvent`, `ProgressEvent`, `LogEvent`, `CompletedEvent`, `FailedEvent`
- `IFfmpegSession` interface for consumer mocking
- CliWrap-based process management (cross-platform, deadlock-free)
- `Ffprobe.AnalyseAsync` with System.Text.Json source-generated parsing
- Simple and complex filter support (Scale, Crop, Fade, FilterGraph DAG)
- Pipe I/O via `PipeSource.FromStream` / `PipeTarget.ToStream`
- Multi-output and tee muxer support
- Graceful cancellation (q on stdin / SIGINT fallback)
- FFMetadata chapter builder
