# integration-tests Specification

## Purpose
Integration tests verifying real FFmpeg execution — transcoding, remuxing, events, cancellation, errors, probing, filters, pipe I/O, multi-output, containers, and metadata.
## Requirements
### Requirement: Basic transcode tests
Tests SHALL verify real transcoding: H264→H264 with CRF, stream copy (remux), format conversion (MP4→MKV), audio transcode (AAC→AAC with different bitrate).

#### Scenario: H264 transcode with CRF
- **WHEN** a session transcodes `video_h264_aac_5s.mp4` with VideoEncode(H264, Crf=28) and AudioEncode(AAC, 192kbps)
- **THEN** the output file has an H264 video stream, AAC audio stream, and duration ~5s

#### Scenario: Stream copy remux
- **WHEN** a session copies all streams from MP4 to MKV
- **THEN** the output is a valid MKV with the same streams

### Requirement: Audio transcode tests
Tests SHALL verify audio-only transcoding: bitrate change, sample rate change, channel downmix.

#### Scenario: Audio bitrate change
- **WHEN** a session transcodes audio with Bitrate.Kbps(64)
- **THEN** the output audio stream exists and file is smaller than the original

### Requirement: Subtitle mux tests
Tests SHALL verify muxing an external SRT file into a container.

#### Scenario: SRT mux into MKV
- **WHEN** a session maps video+audio from MP4 and subtitle from SRT into MKV
- **THEN** the output MKV has a subtitle stream

### Requirement: Event stream tests
Tests SHALL verify the Observable event lifecycle: StartedEvent is first, ProgressEvent(s) arrive during processing, CompletedEvent or FailedEvent is terminal.

#### Scenario: Successful transcode event order
- **WHEN** a session transcodes a valid file
- **THEN** events arrive in order: StartedEvent, zero or more ProgressEvent, CompletedEvent

#### Scenario: Failed transcode event order
- **WHEN** a session processes a corrupt file
- **THEN** events arrive: StartedEvent, FailedEvent with non-zero exit code

### Requirement: Cancellation tests
Tests SHALL verify graceful cancellation: CancellationToken triggers 'q' on stdin, process exits within timeout.

#### Scenario: Cancel during transcode
- **WHEN** a session is cancelled via CancellationToken during a transcode
- **THEN** the result indicates cancellation and the process is not left running

### Requirement: Error handling tests
Tests SHALL verify error scenarios: corrupt input file, missing input file, invalid codec.

#### Scenario: Corrupt input
- **WHEN** a session processes `corrupt.mp4`
- **THEN** RunAsync returns a failed result with FailureKind.Permanent

#### Scenario: Missing input file
- **WHEN** a session references a non-existent file
- **THEN** RunAsync returns a failed result

### Requirement: Probing tests
Tests SHALL verify `Ffprobe.AnalyseAsync` against known test files.

#### Scenario: Probe video file
- **WHEN** `Ffprobe.AnalyseAsync` is called on `video_h264_aac_5s.mp4`
- **THEN** MediaInfo has one VideoStreamInfo (H264, 320x240) and one AudioStreamInfo (AAC)

#### Scenario: Probe audio-only file
- **WHEN** `Ffprobe.AnalyseAsync` is called on `audio_only_3s.m4a`
- **THEN** MediaInfo has zero VideoStreamInfo and one AudioStreamInfo

### Requirement: Filter integration tests
Tests SHALL verify that filters produce correct output by probing the result.

#### Scenario: Scale filter
- **WHEN** a session applies Scale(160, 120) to `video_h264_aac_5s.mp4`
- **THEN** the output file has resolution 160x120

#### Scenario: Crop filter
- **WHEN** a session applies Crop(200, 200) to `video_h264_aac_5s.mp4`
- **THEN** the output file has resolution 200x200

### Requirement: Pipe I/O tests (Windows)
Tests SHALL verify named pipe input and output on Windows.

#### Scenario: Pipe input
- **WHEN** a MemoryStream is piped as input via IPipeSource
- **THEN** ffmpeg reads from the named pipe and produces output

### Requirement: Multi-output tests
Tests SHALL verify multiple output files from a single session.

#### Scenario: Two outputs with different CRF
- **WHEN** a session outputs the same input to two files with CRF 18 and CRF 28
- **THEN** both output files exist and the CRF-28 file is smaller

### Requirement: Container format tests
Tests SHALL verify output to different containers.

#### Scenario: Output to MKV
- **WHEN** a session transcodes to MKV with Container(ContainerFormat.Mkv)
- **THEN** the output is a valid Matroska file

### Requirement: Metadata tests
Tests SHALL verify metadata operations on real files.

#### Scenario: Strip metadata
- **WHEN** a session strips metadata with MetadataSpec(StripAll: true)
- **THEN** the output file has no global metadata entries

### Requirement: Tee muxer integration test
A test SHALL verify that tee muxer produces multiple output files from a single ffmpeg process.

#### Scenario: Tee split to two files
- **WHEN** a tee output with two targets is executed
- **THEN** both output files exist and contain valid media

### Requirement: Multi-output integration test
A test SHALL verify that multiple outputs from a single process produce correct files.

#### Scenario: Two outputs different quality
- **WHEN** a multi-output session encodes to CRF 18 and CRF 28
- **THEN** both files exist and the CRF-28 file is smaller

### Requirement: Null output integration test
A test SHALL verify the analysis pattern (null output with filter detection).

#### Scenario: Null output with audio filter
- **WHEN** a session outputs to null with a silence detection filter
- **THEN** the session completes successfully

### Requirement: Fluent API end-to-end integration test
A test SHALL verify the full `Ffmpeg.Input().Output()` fluent path with real ffmpeg.

#### Scenario: Fluent transcode
- **WHEN** `Ffmpeg.Input(file).Output(path, o => o.Stream(...).Encode(...))` runs
- **THEN** the output file is a valid media file

