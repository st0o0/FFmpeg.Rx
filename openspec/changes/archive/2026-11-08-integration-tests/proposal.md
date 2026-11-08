## Summary

Add a dedicated `FFmpeg.Rx.IntegrationTests` project that runs real ffmpeg processes against synthetic test media checked into the repository. Tests verify actual transcoding, event streams, probing, filters, pipe I/O, and error handling — not just CLI string output.

## Motivation

The existing test suite (186 tests in `FFmpeg.Rx.Tests`) validates the typed domain model, serializer output, and builder correctness — all without running ffmpeg. This catches API bugs but misses integration failures: broken progress parsing against real output, pipe I/O race conditions, graceful cancellation timing, filter chains that produce wrong results.

WebSocket.Rx solves this with a separate `IntegrationTests` project that runs full client↔server round-trips. FFmpeg.Rx needs the same pattern, adapted for an external process instead of an in-process server.

## Goals

- Test real ffmpeg process execution end-to-end
- Verify event stream order (Started → Progress* → Completed/Failed)
- Validate output files via ffprobe (resolution, codec, duration)
- Test graceful cancellation, error handling, pipe I/O
- Ship small synthetic test media in the repo (CC0, ~260KB total)
- Keep tests fast enough for CI (<60s total)

## Non-Goals

- Exhaustive codec/format matrix testing (ffmpeg's own test suite covers that)
- HW acceleration tests (GPU-dependent, not portable)
- Performance benchmarks
- Testing ffprobe JSON parsing (already covered in unit tests)
