## 1. Codec & Encoding Foundations

- [x] 1.1 Create `VideoCodec` static class with string constants (LibX264, LibX265, LibVpx, LibVpxVp9, LibAomAv1, LibSvtAv1, H264Nvenc, HevcNvenc, H264Qsv, Copy)
- [x] 1.2 Create `AudioCodec` static class with string constants (Aac, LibOpus, LibMp3Lame, Flac, Pcm16Le, Copy)
- [x] 1.3 Create `SubtitleCodec` static class with string constants (MovText, Srt, Ass, WebVtt, Copy)
- [x] 1.4 Add `.Crf(int)`, `.VideoBitrate(string)`, `.AudioBitrate(string)` to `OutputOptions`
- [x] 1.5 Add `.Framerate(double)`, `.Framerate(string)` to `OutputOptions`
- [x] 1.6 Create `Preset` static class and add `.Preset(string)` to `OutputOptions`
- [x] 1.7 Create `VideoProfile` static class and add `.Profile(string)` to `OutputOptions`
- [x] 1.8 Create `PixelFormat` static class and add `.PixelFormat(string)` to `OutputOptions`
- [x] 1.9 Add `.ForceFormat(string)` to `OutputOptions`
- [x] 1.10 Add `.NoOverwrite()` to `OutputOptions`, change ArgumentBuilder to emit `-y` by default and `-n` when set
- [x] 1.11 Create `EncoderTune` static class and add `.Tune(string)` to `OutputOptions`
- [x] 1.12 Update `ArgumentBuilder` to serialize all new encoding options
- [x] 1.13 Write unit tests for all codec constants and encoding option serialization

## 2. ffprobe / Media Analysis

- [x] 2.1 Create `MediaInfo`, `VideoStreamInfo`, `AudioStreamInfo`, `SubtitleStreamInfo`, `ChapterInfo` immutable records
- [x] 2.2 Create `FfprobeOptions` record with `BinaryPath` property
- [x] 2.3 Create `FfprobeException` with `ExitCode` and `StandardError`
- [x] 2.4 Create `ProbeResultParser` using System.Text.Json source generators to parse ffprobe JSON into `MediaInfo`
- [x] 2.5 Create `ProbeRunner` (internal) that runs ffprobe with `-print_format json -show_format -show_streams -show_chapters`
- [x] 2.6 Create `Ffprobe` static entry with `AnalyseAsync(string path, ...)` and `AnalyseAsync(Uri url, ...)`
- [x] 2.7 Write unit tests for `ProbeResultParser` with sample ffprobe JSON fixtures
- [x] 2.8 Write integration tests for `Ffprobe.AnalyseAsync` with a real video file

## 3. Percentage Progress & Graceful Cancellation

- [x] 3.1 Add `PercentageProgressEvent` record and `AsPercentage(TimeSpan totalDuration)` extension on `ProgressEvent`
- [x] 3.2 Create `WithPercentage(TimeSpan)` R3 operator extension on `Observable<ProgressEvent>`
- [x] 3.3 Change `ProcessRunner` to set `RedirectStandardInput = true`
- [x] 3.4 Implement graceful cancellation: write `q` to stdin, wait 5s, then kill
- [x] 3.5 Write unit tests for percentage calculation (including zero duration edge case)
- [x] 3.6 Write integration test for graceful cancellation producing valid output

## 4. Time Control

- [x] 4.1 Add `.Seek(TimeSpan)` to `InputOptions` (emits before `-i`)
- [x] 4.2 Add `.Seek(TimeSpan)`, `.Duration(TimeSpan)`, `.StopTime(TimeSpan)` to `OutputOptions`
- [x] 4.3 Add `.Loop()` and `.StreamLoop(int)` to `InputOptions`
- [x] 4.4 Add `.VideoSpeed(double)` and `.AudioTempo(double)` to `OutputOptions` with atempo chaining for values outside 0.5-2.0
- [x] 4.5 Update `ArgumentBuilder` to serialize all time control options with correct placement (input vs output)
- [x] 4.6 Write unit tests for time control argument serialization including atempo chaining

## 5. Simple Filters

- [x] 5.1 Define `IVideoFilter` and `IAudioFilter` interfaces with `string Render()` method
- [x] 5.2 Create video filter records: `ScaleFilter`, `CropFilter`, `HFlipFilter`, `VFlipFilter`, `TransposeFilter`, `FadeFilter`, `PadFilter`, `YadifFilter`
- [x] 5.3 Create audio filter records: `VolumeFilter`, `LoudNormFilter`, `AResampleFilter`, `SilenceDetectFilter`
- [x] 5.4 Create `CustomFilter` record implementing both interfaces
- [x] 5.5 Create `VideoFilterBuilder` with fluent methods returning `this`, producing `IReadOnlyList<IVideoFilter>`
- [x] 5.6 Create `AudioFilterBuilder` with fluent methods returning `this`, producing `IReadOnlyList<IAudioFilter>`
- [x] 5.7 Add `.VideoFilter(Action<VideoFilterBuilder>)` and `.AudioFilter(Action<AudioFilterBuilder>)` to `OutputOptions`
- [x] 5.8 Update `ArgumentBuilder` to serialize filter chains as `-vf "..."` / `-af "..."`
- [x] 5.9 Write unit tests for each filter's `Render()` output
- [x] 5.10 Write unit tests for chained filter serialization

## 6. Complex Filter Graphs

- [x] 6.1 Create `FilterGraph` immutable record model (inputs, chains, merges, splits)
- [x] 6.2 Create `FilterGraphBuilder` with `.Input()`, `.Chain()`, `.Merge()`, `.Split()`, `.ASplit()`
- [x] 6.3 Create `FilterChainBuilder` (reuses video filter records from Phase 5)
- [x] 6.4 Create `MergeFilterBuilder` with `.Overlay()`, `.Concat()`, `.AudioMix()`
- [x] 6.5 Add `.BurnSubtitles(string path)` to `FilterChainBuilder`
- [x] 6.6 Add `.FilterComplex(FilterGraph)` and `.MapLabel(string)` to `OutputOptions`
- [x] 6.7 Update `ArgumentBuilder` to serialize `FilterGraph` as `-filter_complex "..."` string
- [x] 6.8 Write unit tests for graph serialization (single chain, multi-input merge, split, concat)

## 7. Pipe I/O

- [x] 7.1 Define `IPipeSource` and `IPipeSink` interfaces
- [x] 7.2 Create `StreamPipeSource` and `StreamPipeSink` implementations
- [x] 7.3 Create `PipeInput` record extending `FfmpegInput`
- [x] 7.4 Create `PipeOutput` record
- [x] 7.5 Add `.Input(IPipeSource, Action<InputOptions>?)` to `PipelineBuilder`
- [x] 7.6 Add `.OutputPipe(IPipeSink, Action<OutputOptions>)` to `PipelineBuilder`
- [x] 7.7 Implement Windows named pipe management in `ProcessRunner`
- [x] 7.8 Implement Unix pipe management in `ProcessRunner`
- [x] 7.9 Update `ArgumentBuilder` to emit pipe paths/protocols
- [x] 7.10 Write integration tests for pipe input and pipe output

## 8. Multiple Outputs

- [x] 8.1 Extend `FfmpegPipeline` to hold `IReadOnlyList<FfmpegOutput>` instead of single `FfmpegOutput`
- [x] 8.2 Change `PipelineBuilder` to allow chaining multiple `.Output()` calls
- [x] 8.3 Create `TeeBuilder` and `TeeTarget` records
- [x] 8.4 Add `.OutputTee(Action<TeeBuilder>)` to `PipelineBuilder`
- [x] 8.5 Add `.OutputNull(Action<OutputOptions>)` to `PipelineBuilder`
- [x] 8.6 Update `ArgumentBuilder` to serialize multiple outputs and tee muxer
- [x] 8.7 Write unit tests for multi-output and tee argument serialization

## 9. Stream Recording

- [x] 9.1 Verify existing URL input + stream copy + duration works for RTMP/HLS/RTSP recording
- [x] 9.2 Write integration test recording a short HLS stream to file
- [x] 9.3 Add documentation/examples for stream recording patterns

## 10. Metadata & Chapters

- [x] 10.1 Add `.StreamMetadata(string key, string value, StreamType, int?)` to `OutputOptions`
- [x] 10.2 Add `.GlobalMetadata(string key, string value)` to `OutputOptions`
- [x] 10.3 Add `.WithoutMetadata()` and `.CopyMetadataFrom(int)` to `OutputOptions`
- [x] 10.4 Add `.Disposition(StreamType, int, params string[])` to `OutputOptions`
- [x] 10.5 Create `FFMetadataBuilder` with `.WithTitle()`, `.WithChapter()`, `.Build()`
- [x] 10.6 Add `.AddMetadata(FFMetadataBuilder)` to `PipelineBuilder`
- [x] 10.7 Update `ArgumentBuilder` for metadata, disposition, and ffmetadata serialization
- [x] 10.8 Write unit tests for metadata serialization and FFMetadataBuilder output format

## 11. Subtitle Handling

- [x] 11.1 Verify subtitle mux works via existing `.Map()` with `SubtitleCodec` constants
- [x] 11.2 Verify subtitle extraction works via mapping only subtitle streams
- [x] 11.3 Verify subtitle burn-in works via complex filter `.BurnSubtitles()` from Phase 6
- [x] 11.4 Write integration tests for subtitle mux, extract, and burn-in

## 12. Hardware Acceleration

- [x] 12.1 Create `HWAccel` static class with constants (Cuda, Qsv, Dxva2, D3d11va, VideoToolbox, Vaapi, Auto)
- [x] 12.2 Add `.HWAccel(string)`, `.HWAccelDevice(string)`, `.HWAccelOutputFormat(string)` to `InputOptions`
- [x] 12.3 Add `.VideoDecoder(string)` to `InputOptions`
- [x] 12.4 Add HW encoder constants to `VideoCodec` (H264Nvenc, HevcNvenc, H264Qsv, HevcQsv, H264Amf, HevcAmf, H264VideoToolbox, HevcVideoToolbox)
- [x] 12.5 Update `ArgumentBuilder` to emit HW accel arguments before `-i`
- [x] 12.6 Write unit tests for HW acceleration argument serialization

## 13. Global Configuration

- [x] 13.1 Create `FfmpegOptions` record with `BinaryPath`, `FfprobeBinaryPath`, `TempDirectory`, `WorkingDirectory`
- [x] 13.2 Create `FfmpegOptions.Configure(FfmpegOptions)` static method for global config
- [x] 13.3 Update `ProcessRunner` and `ProbeRunner` to resolve binary from global config → per-session override → PATH
- [x] 13.4 Implement binary discovery from common platform locations as fallback
- [x] 13.5 Update `ProcessRunner` to set `WorkingDirectory` on `ProcessStartInfo`
- [x] 13.6 Write unit tests for binary resolution priority and config

## 14. Ecosystem Extras

- [x] 14.1 Add `.NegativeMap(int, StreamType)` to `OutputOptions`
- [x] 14.2 Add `.CodecTag(string, StreamType)` to `OutputOptions`
- [x] 14.3 Add `.MovFlags(params string[])` to `OutputOptions`
- [x] 14.4 Add `.MapAll(int)` to `OutputOptions`
- [x] 14.5 Add `.CopyStreams()` to `OutputOptions`
- [x] 14.6 Add `.Framerate(double)` to `InputOptions` for image sequences
- [x] 14.7 Update `ArgumentBuilder` for all ecosystem extras serialization
- [x] 14.8 Write unit tests for all ecosystem extras argument serialization
