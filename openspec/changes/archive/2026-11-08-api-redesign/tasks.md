## 1. Project Infrastructure

- [x] 1.1 Create AGENTS.md with project description, solution structure, architecture, guardrails, C# conventions, test conventions (assertion rules, timeouts), build/test commands
- [x] 1.2 Update CLAUDE.md to start with `@AGENTS.md` and add skill routing table
- [x] 1.3 Fill openspec/config.yaml with real context (tech stack, architecture, conventions) and rules
- [x] 1.4 Add `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` to Directory.Build.props and fix all existing warnings
- [x] 1.5 Fix ci.yml to reference `src/FFmpeg.Rx.slnx` instead of template placeholder

## 2. Value Objects

- [x] 2.1 Create `VideoCodec` readonly record struct with static factories (H264, Hevc, Vp9, Av1, H264Nvenc, HevcNvenc, H264Qsv, HevcQsv, H264Cuvid, Copy) and `Custom()`
- [x] 2.2 Create `AudioCodec` readonly record struct with static factories (Aac, Opus, Mp3, Flac, Pcm16Le, Copy) and `Custom()`
- [x] 2.3 Create `SubtitleCodec` readonly record struct with static factories (MovText, Srt, Ass, WebVtt, Copy) and `Custom()`
- [x] 2.4 Create `Bitrate` readonly record struct with `BitsPerSecond`, `Kbps()`, `Mbps()`, `ToFfmpegString()`
- [x] 2.5 Create `Framerate` readonly record struct with `Numerator`/`Denominator`, `Fps()`, `Rational()`, NTSC constants
- [x] 2.6 Create `QualityConfig` hierarchy: abstract `QualityConfig`, `ConstantQuality(int Crf)`, `ConstantBitrate(Bitrate Rate)`, plus `Quality` static factory class
- [x] 2.7 Create `Preset` readonly record struct with static factories and `Custom()`
- [x] 2.8 Create `VideoProfile` readonly record struct with static factories and `Custom()`
- [x] 2.9 Create `Tune` readonly record struct with static factories and `Custom()`
- [x] 2.10 Create `PixelFormat` readonly record struct with static factories and `Custom()`
- [x] 2.11 Create `ContainerFormat` readonly record struct with static factories (Mp4, Mkv, WebM, Ts, Flv, Null) and `Custom()`
- [x] 2.12 Update `HWAccel` from static class to readonly record struct with `Custom()`
- [x] 2.13 Write unit tests for all value objects: equality, factories, ffmpeg string output

## 3. Typed Pipeline Model

- [x] 3.1 Create `PipelineConfig` record replacing `FfmpegPipeline`
- [x] 3.2 Create `InputConfig` abstract record with `FileInput`, `UrlInput`, `PipeInput` subtypes carrying typed `InputOptions` record
- [x] 3.3 Create `OutputConfig` record with typed fields: Destination, Streams, Container, Filters, Metadata, TimeRange, Overwrite, FastStart, MovFlags, CustomArgs
- [x] 3.4 Create `StreamConfig` record with InputIndex, StreamIndex, Type, CodecConfig, Metadata, Disposition
- [x] 3.5 Create `CodecConfig` hierarchy: abstract `CodecConfig`, `StreamCopy`, `VideoEncode`, `AudioEncode`, `SubtitleEncode`
- [x] 3.6 Create `VideoEncodingOptions` record with Quality, Preset, Profile, Tune, PixelFormat, Framerate
- [x] 3.7 Create `AudioEncodingOptions` record with Bitrate, SampleRate, Channels
- [x] 3.8 Create `TimeRange` record with Seek, Duration, StopTime
- [x] 3.9 Create `MetadataConfig` record with GlobalEntries, StripAll, CopyFromInput
- [x] 3.10 Create `FilterConfig` hierarchy: `SimpleFilterConfig`, `ComplexFilterConfig`
- [x] 3.11 Create `OutputDestination` hierarchy: `FileDestination`, `PipeDestination`, `NullDestination`
- [x] 3.12 Remove old `FfmpegPipeline`, `FfmpegOutput`, `StreamMapping` records and old `Codecs/` static classes
- [x] 3.13 Write unit tests verifying record construction and properties

## 4. Scoped Builders

- [x] 4.1 Rewrite `OutputBuilder` (replaces OutputOptions) producing `OutputConfig` — scoped methods only
- [x] 4.2 Create `StreamBuilder` with `.Copy()` (terminal) and `.Encode(VideoCodec)`, `.Encode(AudioCodec)`, `.Encode(SubtitleCodec)` forks
- [x] 4.3 Create `VideoEncodingBuilder` with `.Quality()`, `.Preset()`, `.Profile()`, `.Tune()`, `.PixelFormat()`, `.Framerate()`
- [x] 4.4 Create `AudioEncodingBuilder` with `.Bitrate()`, `.SampleRate()`, `.Channels()`
- [x] 4.5 Create `TimeRangeBuilder` producing `TimeRange`
- [x] 4.6 Create `MetadataBuilder` producing `MetadataConfig`
- [x] 4.7 Update `PipelineBuilder` to use new `OutputBuilder` and produce `PipelineConfig`
- [x] 4.8 Update `Ffmpeg` static entry to work with new builders
- [x] 4.9 Update `MultiOutputBuilder` for new OutputConfig
- [x] 4.10 Add `.CustomArg()` escape hatch on every builder
- [x] 4.11 Write unit tests: builder produces correct typed records for all scenarios (transcode, remux, HW accel, filters, multi-output, tee, null output)

## 5. Session Interface

- [x] 5.1 Create `IFfmpegSession` interface with Events, Progress, Log, RunAsync
- [x] 5.2 Update `FfmpegSession` to implement `IFfmpegSession`
- [x] 5.3 Update `PipelineBuilder.Output()` and `MultiOutputBuilder.Build()` to return `IFfmpegSession`
- [x] 5.4 Update `FfmpegSession.Create()` to accept `PipelineConfig`
- [x] 5.5 Write unit test verifying interface contract

## 6. Argument Serializer

- [x] 6.1 Rename `ArgumentBuilder` to `ArgumentSerializer`
- [x] 6.2 Rewrite `Serialize(PipelineConfig)` to decompose into per-concern methods using pattern matching on typed records
- [x] 6.3 Implement `SerializeCodec` with pattern matching: StreamCopy, VideoEncode, AudioEncode, SubtitleEncode
- [x] 6.4 Implement `SerializeVideoEncoding` for QualityConfig, Preset, Profile, Tune, PixelFormat, Framerate
- [x] 6.5 Implement `SerializeAudioEncoding` for Bitrate, SampleRate, Channels
- [x] 6.6 Implement `SerializeTimeRange` for Seek, Duration, StopTime
- [x] 6.7 Implement `SerializeMetadata` for global entries, strip all, copy from
- [x] 6.8 Implement `SerializeFilters` (reuse existing filter Render() methods)
- [x] 6.9 Implement `SerializeInput` with InputOptions (HWAccel, Decoder, Seek)
- [x] 6.10 Implement multi-output and tee serialization
- [x] 6.11 Write unit tests: known typed records → expected CLI strings for every feature

## 7. Update ProcessRunner

- [x] 7.1 Update `ProcessRunner` to accept `PipelineConfig` instead of old `FfmpegPipeline`
- [x] 7.2 Update pipe management for new `PipeInput`/`PipeDestination` types
- [x] 7.3 Verify graceful cancellation still works with new model

## 8. Migrate Tests

- [x] 8.1 Rewrite `ArgumentBuilderTests` → `ArgumentSerializerTests` asserting CLI output from typed records
- [x] 8.2 Rewrite `EncodingOptionsTests` to assert on domain records, not string matching
- [x] 8.3 Rewrite `PipelineBuilderTests` to assert builder produces correct PipelineConfig
- [x] 8.4 Rewrite `TimeControlTests` with typed TimeRange assertions
- [x] 8.5 Rewrite `MetadataTests` with typed MetadataConfig assertions
- [x] 8.6 Rewrite `HWAccelTests` with typed InputOptions assertions
- [x] 8.7 Rewrite `EcosystemExtrasTests` with typed record assertions
- [x] 8.8 Rewrite `MultiOutputTests` with typed OutputConfig assertions
- [x] 8.9 Fix all vacuous `Assert.NotNull(session)` assertions to test actual behavior
- [x] 8.10 Add inline timeouts to all async tests
- [x] 8.11 Remove old test files that tested the string-based API
- [x] 8.12 Verify filter tests and probe tests still pass (these should be mostly unchanged)
- [x] 8.13 Final: all tests green, `dotnet format --verify-no-changes`, zero warnings
