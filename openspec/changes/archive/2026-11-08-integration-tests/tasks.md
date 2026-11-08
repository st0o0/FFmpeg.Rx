## 1. Test Media

- [x] 1.1 Create `testdata/` directory at repo root
- [x] 1.2 Write `testdata/generate-testdata.sh` script that generates all test files using ffmpeg lavfi
- [x] 1.3 Run the script to generate all test files
- [x] 1.4 Generate `video_h264_aac_5s.mp4` (testsrc 320x240 30fps + sine 440Hz, H264+AAC, 5s)
- [x] 1.5 Generate `video_only_3s.mp4` (testsrc 320x240, H264, 3s, no audio)
- [x] 1.6 Generate `audio_only_3s.m4a` (sine 440Hz, AAC, 3s)
- [x] 1.7 Generate `video_hevc_3s.mp4` (testsrc 320x240, HEVC, 3s)
- [x] 1.8 Create `subtitle.srt` with 3 timed entries covering 0-5 seconds
- [x] 1.9 Generate `two_audio_tracks.mkv` (testsrc + sine 440Hz + sine 880Hz, H264+2xAAC, 3s)
- [x] 1.10 Create `corrupt.mp4` (first 512 bytes of `video_h264_aac_5s.mp4`)
- [x] 1.11 Write `testdata/README.md` with generation docs and CC0 license statement
- [x] 1.12 Add `testdata/` to `.gitattributes` as binary (prevent line-ending conversion)

## 2. Test Infrastructure

- [x] 2.1 Create `FFmpeg.Rx.IntegrationTests` project (xUnit v3, MTP, reference FFmpeg.Rx)
- [x] 2.2 Add project to `FFmpeg.Rx.slnx`
- [x] 2.3 Create `Internal/TestPaths.cs` — resolves `testdata/` path relative to project, provides temp output dir factory
- [x] 2.4 Create `Internal/FfmpegTestBase.cs` — IAsyncLifetime, temp dir per test, TestAsset/OutputPath helpers
- [x] 2.5 Create `Internal/FfmpegAssert.cs` — probing-based assertions using Ffprobe.AnalyseAsync
- [x] 2.6 Create `Internal/FfmpegFixture.cs` — IAsyncLifetime class fixture, checks ffmpeg availability once, skips all tests if missing
- [x] 2.7 Verify build: `dotnet build src/FFmpeg.Rx.slnx` compiles both test projects

## 3. Transcode Tests

- [x] 3.1 `BasicTranscodeTests.Transcode_H264_with_Crf` — encode with CRF, probe output codec+duration
- [x] 3.2 `BasicTranscodeTests.Stream_copy_remux_Mp4_to_Mkv` — copy streams, probe output container
- [x] 3.3 `BasicTranscodeTests.Format_conversion_with_container` — use ContainerFormat, probe output
- [x] 3.4 `AudioTranscodeTests.Transcode_audio_bitrate` — AAC with Bitrate.Kbps(64), probe output
- [x] 3.5 `AudioTranscodeTests.Audio_only_transcode` — audio-only input, probe output
- [x] 3.6 `SubtitleMuxTests.Mux_Srt_into_Mkv` — map video+audio+subtitle, probe subtitle stream exists

## 4. Event Stream Tests

- [x] 4.1 `ProgressEventTests.Successful_transcode_emits_correct_event_order` — Started→Progress*→Completed
- [x] 4.2 `ProgressEventTests.Progress_events_have_valid_data` — frame count, speed, bitrate populated
- [x] 4.3 `ErrorHandlingTests.Corrupt_input_emits_FailedEvent` — corrupt.mp4 → FailedEvent
- [x] 4.4 `ErrorHandlingTests.Missing_input_file_fails` — nonexistent path → failed result
- [x] 4.5 `CancellationTests.Cancel_during_transcode` — cancel token, verify process exits

## 5. Probing Tests

- [x] 5.1 `ProbeVideoTests.Probe_video_h264_aac` — probe known file, assert streams, resolution, duration
- [x] 5.2 `ProbeVideoTests.Probe_hevc_video` — probe HEVC file, assert codec name
- [x] 5.3 `ProbeAudioTests.Probe_audio_only` — probe audio file, assert no video stream
- [x] 5.4 `ProbeVideoTests.Probe_two_audio_tracks` — probe MKV with 2 audio streams

## 6. Filter Tests

- [x] 6.1 `ScaleFilterTests.Scale_down_resolution` — Scale(160,120), probe output resolution
- [x] 6.2 `CropFilterTests.Crop_to_square` — Crop(200,200), probe output resolution

## 7. Pipe I/O Tests

- [x] 7.1 `PipeInputTests.Stream_to_pipe_input` — MemoryStream as IPipeSource, verify output created (Windows only)
- [x] 7.2 `PipeOutputTests.Pipe_output_to_stream` — ffmpeg output to IPipeSink, verify bytes received (Windows only)

## 8. Advanced Tests

- [x] 8.1 `MultiOutputTests.Two_outputs_different_quality` — two outputs, both exist, size differs
- [x] 8.2 `ContainerFormatTests.Output_to_Mkv` — ContainerFormat.Mkv, probe Matroska
- [x] 8.3 `ContainerFormatTests.Output_to_Ts` — ContainerFormat.Ts, verify MPEG-TS output
- [x] 8.4 `MetadataTests.Strip_all_metadata` — MetadataConfig(StripAll: true), probe no metadata
- [x] 8.5 `MetadataTests.Add_global_metadata` — add title, probe it back

## 9. Finalize

- [x] 9.1 All integration tests green with ffmpeg installed
- [x] 9.2 All integration tests skip cleanly without ffmpeg
- [x] 9.3 `dotnet build src/FFmpeg.Rx.slnx` — zero errors, zero warnings
- [x] 9.4 `dotnet format src/FFmpeg.Rx.slnx --verify-no-changes` — clean
- [x] 9.5 Update AGENTS.md with IntegrationTests project in solution structure
