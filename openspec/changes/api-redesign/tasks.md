## 1. Project Infrastructure

- [ ] 1.1 Create AGENTS.md with project description, solution structure, architecture, guardrails, C# conventions, test conventions (assertion rules, timeouts), build/test commands
- [ ] 1.2 Update CLAUDE.md to start with `@AGENTS.md` and add skill routing table
- [ ] 1.3 Fill openspec/config.yaml with real context (tech stack, architecture, conventions) and rules
- [ ] 1.4 Add `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` to Directory.Build.props and fix all existing warnings
- [ ] 1.5 Fix ci.yml to reference `src/FFmpeg.Rx.slnx` instead of template placeholder

## 2. Value Objects

- [ ] 2.1 Create `VideoCodec` readonly record struct with static factories (H264, Hevc, Vp9, Av1, H264Nvenc, HevcNvenc, H264Qsv, HevcQsv, H264Cuvid, Copy) and `Custom()`
- [ ] 2.2 Create `AudioCodec` readonly record struct with static factories (Aac, Opus, Mp3, Flac, Pcm16Le, Copy) and `Custom()`
- [ ] 2.3 Create `SubtitleCodec` readonly record struct with static factories (MovText, Srt, Ass, WebVtt, Copy) and `Custom()`
- [ ] 2.4 Create `Bitrate` readonly record struct with `BitsPerSecond`, `Kbps()`, `Mbps()`, `ToFfmpegString()`
- [ ] 2.5 Create `Framerate` readonly record struct with `Numerator`/`Denominator`, `Fps()`, `Rational()`, NTSC constants
- [ ] 2.6 Create `QualitySpec` hierarchy: abstract `QualitySpec`, `ConstantQuality(int Crf)`, `ConstantBitrate(Bitrate Rate)`, plus `Quality` static factory class
- [ ] 2.7 Create `Preset` readonly record struct with static factories and `Custom()`
- [ ] 2.8 Create `VideoProfile` readonly record struct with static factories and `Custom()`
- [ ] 2.9 Create `Tune` readonly record struct with static factories and `Custom()`
- [ ] 2.10 Create `PixelFormat` readonly record struct with static factories and `Custom()`
- [ ] 2.11 Create `ContainerFormat` readonly record struct with static factories (Mp4, Mkv, WebM, Ts, Flv, Null) and `Custom()`
- [ ] 2.12 Update `HWAccel` from static class to readonly record struct with `Custom()`
- [ ] 2.13 Write unit tests for all value objects: equality, factories, ffmpeg string output

## 3. Typed Pipeline Model

- [ ] 3.1 Create `PipelineSpec` record replacing `FfmpegPipeline`
- [ ] 3.2 Create `InputSpec` abstract record with `FileInput`, `UrlInput`, `PipeInput` subtypes carrying typed `InputOptions` record
- [ ] 3.3 Create `OutputSpec` record with typed fields: Destination, Streams, Container, Filters, Metadata, TimeRange, Overwrite, FastStart, MovFlags, CustomArgs
- [ ] 3.4 Create `StreamSpec` record with InputIndex, StreamIndex, Type, CodecSpec, Metadata, Disposition
- [ ] 3.5 Create `CodecSpec` hierarchy: abstract `CodecSpec`, `StreamCopy`, `VideoEncode`, `AudioEncode`, `SubtitleEncode`
- [ ] 3.6 Create `VideoEncodingOptions` record with Quality, Preset, Profile, Tune, PixelFormat, Framerate
- [ ] 3.7 Create `AudioEncodingOptions` record with Bitrate, SampleRate, Channels
- [ ] 3.8 Create `TimeRange` record with Seek, Duration, StopTime
- [ ] 3.9 Create `MetadataSpec` record with GlobalEntries, StripAll, CopyFromInput
- [ ] 3.10 Create `FilterSpec` hierarchy: `SimpleFilterSpec`, `ComplexFilterSpec`
- [ ] 3.11 Create `OutputDestination` hierarchy: `FileDestination`, `PipeDestination`, `NullDestination`
- [ ] 3.12 Remove old `FfmpegPipeline`, `FfmpegOutput`, `StreamMapping` records and old `Codecs/` static classes
- [ ] 3.13 Write unit tests verifying record construction and properties

## 4. Scoped Builders

- [ ] 4.1 Rewrite `OutputBuilder` (replaces OutputOptions) producing `OutputSpec` — scoped methods only
- [ ] 4.2 Create `StreamBuilder` with `.Copy()` (terminal) and `.Encode(VideoCodec)`, `.Encode(AudioCodec)`, `.Encode(SubtitleCodec)` forks
- [ ] 4.3 Create `VideoEncodingBuilder` with `.Quality()`, `.Preset()`, `.Profile()`, `.Tune()`, `.PixelFormat()`, `.Framerate()`
- [ ] 4.4 Create `AudioEncodingBuilder` with `.Bitrate()`, `.SampleRate()`, `.Channels()`
- [ ] 4.5 Create `TimeRangeBuilder` producing `TimeRange`
- [ ] 4.6 Create `MetadataBuilder` producing `MetadataSpec`
- [ ] 4.7 Update `PipelineBuilder` to use new `OutputBuilder` and produce `PipelineSpec`
- [ ] 4.8 Update `Ffmpeg` static entry to work with new builders
- [ ] 4.9 Update `MultiOutputBuilder` for new OutputSpec
- [ ] 4.10 Add `.CustomArg()` escape hatch on every builder
- [ ] 4.11 Write unit tests: builder produces correct typed records for all scenarios (transcode, remux, HW accel, filters, multi-output, tee, null output)

## 5. Session Interface

- [ ] 5.1 Create `IFfmpegSession` interface with Events, Progress, Log, RunAsync
- [ ] 5.2 Update `FfmpegSession` to implement `IFfmpegSession`
- [ ] 5.3 Update `PipelineBuilder.Output()` and `MultiOutputBuilder.Build()` to return `IFfmpegSession`
- [ ] 5.4 Update `FfmpegSession.Create()` to accept `PipelineSpec`
- [ ] 5.5 Write unit test verifying interface contract

## 6. Argument Serializer

- [ ] 6.1 Rename `ArgumentBuilder` to `ArgumentSerializer`
- [ ] 6.2 Rewrite `Serialize(PipelineSpec)` to decompose into per-concern methods using pattern matching on typed records
- [ ] 6.3 Implement `SerializeCodec` with pattern matching: StreamCopy, VideoEncode, AudioEncode, SubtitleEncode
- [ ] 6.4 Implement `SerializeVideoEncoding` for QualitySpec, Preset, Profile, Tune, PixelFormat, Framerate
- [ ] 6.5 Implement `SerializeAudioEncoding` for Bitrate, SampleRate, Channels
- [ ] 6.6 Implement `SerializeTimeRange` for Seek, Duration, StopTime
- [ ] 6.7 Implement `SerializeMetadata` for global entries, strip all, copy from
- [ ] 6.8 Implement `SerializeFilters` (reuse existing filter Render() methods)
- [ ] 6.9 Implement `SerializeInput` with InputOptions (HWAccel, Decoder, Seek)
- [ ] 6.10 Implement multi-output and tee serialization
- [ ] 6.11 Write unit tests: known typed records → expected CLI strings for every feature

## 7. Update ProcessRunner

- [ ] 7.1 Update `ProcessRunner` to accept `PipelineSpec` instead of old `FfmpegPipeline`
- [ ] 7.2 Update pipe management for new `PipeInput`/`PipeDestination` types
- [ ] 7.3 Verify graceful cancellation still works with new model

## 8. Migrate Tests

- [ ] 8.1 Rewrite `ArgumentBuilderTests` → `ArgumentSerializerTests` asserting CLI output from typed records
- [ ] 8.2 Rewrite `EncodingOptionsTests` to assert on domain records, not string matching
- [ ] 8.3 Rewrite `PipelineBuilderTests` to assert builder produces correct PipelineSpec
- [ ] 8.4 Rewrite `TimeControlTests` with typed TimeRange assertions
- [ ] 8.5 Rewrite `MetadataTests` with typed MetadataSpec assertions
- [ ] 8.6 Rewrite `HWAccelTests` with typed InputOptions assertions
- [ ] 8.7 Rewrite `EcosystemExtrasTests` with typed record assertions
- [ ] 8.8 Rewrite `MultiOutputTests` with typed OutputSpec assertions
- [ ] 8.9 Fix all vacuous `Assert.NotNull(session)` assertions to test actual behavior
- [ ] 8.10 Add inline timeouts to all async tests
- [ ] 8.11 Remove old test files that tested the string-based API
- [ ] 8.12 Verify filter tests and probe tests still pass (these should be mostly unchanged)
- [ ] 8.13 Final: all tests green, `dotnet format --verify-no-changes`, zero warnings
