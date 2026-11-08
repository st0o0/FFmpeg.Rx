## Why

FFmpeg.Rx's current implementation is a string accumulator disguised as a fluent API. `OutputOptions` has 30+ methods that all do `Arguments.Add($"-flag {value}")` — no domain model, no type safety, no difference from calling `.Arg()` directly. Encoding options, codecs, bitrates, and quality settings are raw strings with no validation, no composability, and no testability beyond string matching. The v1-roadmap proved feature coverage is achievable; this change replaces the stringly-typed internals with a real domain model inspired by WebSocket.Rx (interface-first, private-ctor records, C#14 extensions) and FunkArr (enums over strings, typed hierarchies, separated concerns).

## What Changes

- **BREAKING** Replace `OutputOptions` string accumulator with typed `OutputConfig` record containing `StreamConfig[]`, `FilterConfig`, `MetadataConfig`, `TimeRange`, `ContainerFormat`
- **BREAKING** Replace `StreamMapping(int, int, StreamType, string)` with `StreamConfig` carrying a disjoint `CodecConfig` (StreamCopy | VideoEncode | AudioEncode | SubtitleEncode)
- **BREAKING** Replace string codec parameters with `readonly record struct` value types (`VideoCodec`, `AudioCodec`, `SubtitleCodec`) — type-safe between stream types, extensible via `.Custom()`
- **BREAKING** Replace `FfmpegOutput.CustomArguments string[]` with typed record fields — escape hatch moves to `.CustomArg()` on builders
- Add value objects: `Bitrate` (Kbps/Mbps), `Framerate` (Fps/Rational), `Quality` (Crf | Bitrate — disjoint), `Preset`, `VideoProfile`, `Tune`, `PixelFormat`, `ContainerFormat`
- Add scoped builders: `OutputBuilder`, `StreamBuilder`, `VideoEncodingBuilder`, `AudioEncodingBuilder`, `TimeRangeBuilder`, `MetadataBuilder` — each with only the methods that make sense in context
- Add `IFfmpegSession` interface for consumer mocking
- Rename `ArgumentBuilder` to `ArgumentSerializer` — the only place that knows FFmpeg CLI syntax
- Separate video and audio encoding options into distinct types (`VideoEncodingOptions`, `AudioEncodingOptions`)
- Enforce Copy vs Encode at compile time: `s.Copy()` returns no encoding builder, `s.Encode(codec, e => ...)` does
- Add `ContainerFormat` value type with well-known constants (Mp4, Mkv, WebM, Ts, Flv, etc.)
- Add project infrastructure: AGENTS.md, proper OpenSpec config, TreatWarningsAsErrors, fix ci.yml

## Capabilities

### New Capabilities
- `typed-pipeline-model`: Immutable record model for the full FFmpeg pipeline — `PipelineConfig`, `InputConfig`, `OutputConfig`, `StreamConfig`, `CodecConfig`, `EncodingOptions`, `TimeRange`, `MetadataConfig`, `FilterConfig`. Records are the testable truth.
- `value-objects`: Domain value types — `VideoCodec`, `AudioCodec`, `SubtitleCodec`, `Bitrate`, `Framerate`, `Quality`, `Preset`, `VideoProfile`, `Tune`, `PixelFormat`, `ContainerFormat`, `HWAccel`. All `readonly record struct` with static factories and `.Custom()` escape hatches.
- `scoped-builders`: Fluent builder API where each context has its own type — `OutputBuilder`, `StreamBuilder`, `VideoEncodingBuilder`, `AudioEncodingBuilder`, `TimeRangeBuilder`, `MetadataBuilder`. Compile-time enforcement: no CRF on Copy, no Preset on audio, no Bitrate+CRF simultaneously.
- `argument-serializer`: Renamed `ArgumentBuilder` → `ArgumentSerializer`. Pure function: typed records in, string[] out. Single place that knows FFmpeg CLI syntax. Independently testable from domain model.
- `session-interface`: `IFfmpegSession` public interface with `Events`, `Progress`, `Log`, `RunAsync`. Enables consumer mocking and testing without ffmpeg binary.
- `project-infrastructure`: AGENTS.md with guardrails and test conventions, OpenSpec config.yaml with real context/rules, TreatWarningsAsErrors, fixed ci.yml, expanded CLAUDE.md with `@AGENTS.md`.

### Modified Capabilities
<!-- All existing capabilities from v1-roadmap are replaced, not modified — this is a rewrite of the API surface over the same feature set -->

## Impact

- **Public API**: Complete redesign of the builder surface. Every consumer call changes. The reactive event stream (`Observable<FfmpegEvent>`) and probing API (`Ffprobe.AnalyseAsync`) stay the same.
- **Internal**: `ArgumentSerializer` is rewritten to pattern-match on typed records instead of reading string arrays. `ProcessRunner` unchanged. Filter system mostly unchanged (already uses typed records).
- **Tests**: All builder/serialization tests rewritten to assert on domain records, not string matching. Filter tests stay. Probing tests stay.
- **Breaking**: This is a major version bump. The v1-roadmap code was a proof-of-concept; this is the real v1.0 API.
