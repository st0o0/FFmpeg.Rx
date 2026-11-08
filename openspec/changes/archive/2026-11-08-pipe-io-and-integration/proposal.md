## Why

Pipe I/O has a hidden stdin/stdout conflict: progress reporting and pipe output both need stdout, pipe input and graceful cancel both need stdin. The library silently breaks in these combinations. Additionally, tee muxer and multi-output have no integration tests proving they work with real ffmpeg.

## What Changes

### Fix Pipe I/O stdin/stdout conflict

Detect the active mode (File-File, Pipe-File, File-Pipe, Pipe-Pipe) and route progress to stderr when stdout is occupied by pipe output. Use CliWrap's graceful cancel (SIGINT) when stdin is occupied by pipe input.

Progress routing: `-progress pipe:1` (stdout, default) or `-progress pipe:2` (stderr, when stdout is pipe output).

### Integration tests for Tee and Multi-Output

Prove with real ffmpeg that tee split and multi-output actually produce correct files.

### Integration tests for Pipe I/O

Test all four modes with real ffmpeg and real streams.
