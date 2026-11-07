## Why

FFmpeg.Rx has a solid reactive core (R3 Observable event stream, typed event hierarchy, error classification) but lacks the encoding, probing, filtering, and I/O features needed for real-world use. Reference libraries FFMpegCore and FFmpeg.NET cover these — FFmpeg.Rx should reach feature parity with FFMpegCore while keeping the pure-builder, immutable-record architecture and reactive Observable USP.

## What Changes

- Add ffprobe media analysis (`Ffprobe.AnalyseAsync`) with immutable `MediaInfo` record tree
- Add typed codec enums (`VideoCodec`, `AudioCodec`, `SubtitleCodec`) and encoding options (CRF, bitrate, presets, profiles, pixel format, framerate)
- Add percentage progress reporting (requires probe duration)
- Add graceful cancellation (send `q` to ffmpeg instead of kill)
- Add seek/duration/time control on input and output sides
- Add simple filter builders (`VideoFilterBuilder`, `AudioFilterBuilder`) producing immutable filter records
- Add complex filter graph model (`FilterGraph` DAG with labeled pads)
- Add pipe I/O (`IPipeSource`, `IPipeSink`) for in-memory processing
- Add multiple output support and tee muxer
- Add null output for analysis-only runs
- Add stream recording (RTMP/HLS/RTSP capture)
- Add metadata, chapter, disposition, and subtitle handling
- Add hardware acceleration support (CUDA, QSV, DXVA2, D3D11VA, VideoToolbox)
- Add global configuration, binary discovery, and optional binary auto-download
- Add thumbnail sheet, image sequence input, negative mapping, codec tags, movflags

## Capabilities

### New Capabilities
- `ffprobe-analysis`: Media probing via ffprobe — `MediaInfo` record tree with streams, format, chapters, raw JSON
- `codec-encoding`: Typed codec enums, CRF, bitrate, presets, profiles, pixel format, framerate, force format
- `percentage-progress`: Progress as 0-100% using probed duration, plus graceful `q` cancellation
- `time-control`: Seek, duration, stop time on input/output, loop, stream loop, speed/tempo
- `simple-filters`: Video filter chain (-vf) and audio filter chain (-af) with typed filter records
- `complex-filters`: FilterGraph DAG model for -filter_complex — labeled pads, overlay, concat, split, audio mix
- `pipe-io`: Pipe/stream input and output via `IPipeSource`/`IPipeSink`, `StreamPipeSource`/`StreamPipeSink`
- `multi-output`: Multiple outputs from one input, tee muxer, null output
- `stream-recording`: Capture live streams (RTMP, HLS, RTSP) to file
- `metadata-chapters`: Stream metadata, chapter read/write, disposition flags, FFMetadata builder
- `subtitle-handling`: Subtitle mux, extract, and burn-in (via subtitles filter)
- `hw-acceleration`: Hardware acceleration enum, HW decoder/encoder selection, device selection
- `global-config`: Global FFmpegOptions, binary discovery, working directory, optional binary auto-download
- `ecosystem-extras`: Thumbnail sheet, image sequence to video, negative mapping, codec tags, movflags/faststart

### Modified Capabilities
<!-- No existing specs to modify — this is greenfield -->

## Impact

- **Public API surface**: Major expansion — new static entry `Ffprobe`, new enums, new builder methods on `OutputOptions`/`InputOptions`, new `FilterGraph` type, new pipe I/O interfaces, new config types
- **Models**: ~50+ new immutable records across filters, probing, codecs, config
- **Internal**: `ArgumentBuilder` grows significantly to serialize filters, codecs, encoding params; new `ProbeRunner` + `ProbeResultParser` for ffprobe JSON; pipe I/O needs platform-specific named pipe handling
- **Dependencies**: May need `System.Text.Json` for ffprobe output parsing (already in framework)
- **Breaking changes**: None — all additive to current API
- **Test surface**: Each phase needs unit tests for records/builders/parsers and integration tests with real ffmpeg binary
