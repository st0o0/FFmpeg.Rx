## Context

ffmpeg has three standard streams. Pipe I/O and progress reporting compete for them.

## Decisions

### D1: Four execution modes

FfmpegSession detects the mode from PipelineConfig:

| Mode | stdin | stdout | stderr | Progress via |
|------|-------|--------|--------|-------------|
| File-File | cancel (q) | -progress pipe:1 | logs | stdout |
| Pipe-File | pipe input | -progress pipe:1 | logs | stdout |
| File-Pipe | cancel (q) | pipe output | logs + progress | stderr (-progress pipe:2) |
| Pipe-Pipe | pipe input | pipe output | logs + progress | stderr (-progress pipe:2) |

### D2: Progress stderr fallback

When stdout is used for pipe output, `ArgumentSerializer` emits `-progress pipe:2` instead of `-progress pipe:1`. The progress parser reads from stderr events instead.

The `WithProgressReporting()` flag sets a boolean on PipelineConfig. The serializer checks if any output has `PipeDestination` to decide which pipe to use.

Implementation: In `ArgumentSerializer.Serialize`, when emitting global options, detect `-progress pipe:1` and rewrite to `-progress pipe:2` if any output destination is `PipeDestination`.

### D3: Cancel fallback

When stdin is used for pipe input, CliWrap's cancellation sends SIGINT/Ctrl+C. FFmpeg responds to SIGINT by stopping gracefully. No code change needed - CliWrap already handles this.

### D4: ProgressParser handles both stdout and stderr

The progress parser currently only processes stdout lines. When progress comes via stderr (-progress pipe:2), the stderr handler in FfmpegSession needs to also try parsing progress lines before treating them as log events.

In pipe-output mode, stderr carries both logs AND progress. Distinguish by: progress lines match `key=value` pattern (ProgressParser returns non-null), log lines don't.
