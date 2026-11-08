## Why

FfmpegSession has two separate execution methods (ExecuteWithEventStream, ExecuteWithPipeOutput) with duplicated error handling, mutable state scattered across the method (stderrLines, progressBlock, exitCode), and progress/percentage logic inlined in three places. The code grew additively with each feature (pipe I/O, percentage progress, stderr fallback) instead of being refactored.

## What Changes

### Merge two execution paths into one

Replace `ExecuteWithEventStream` and `ExecuteWithPipeOutput` with a single `ExecuteAsync` that takes a mode enum. The mode determines how stdout and stderr are wired, but the event emission, error handling, and result building are shared.

### Extract progress emission as pure function

`EmitProgress(ProgressEvent, TimeSpan?)` returns `IEnumerable<FfmpegEvent>` (the ProgressEvent itself, plus optionally a PercentageProgressEvent). Called from both stdout and stderr handlers.

### Extract stderr classification as pure function

`ClassifyLine(string, Dictionary<string, string>)` returns either a `ProgressEvent` (if the line is a progress key=value) or a `LogEvent`. Used in pipe-output mode where stderr carries both.

### Bundle mutable state into aggregator

Replace scattered `stderrLines`, `progressBlock`, `exitCode` variables with a `SessionState` record that collects stderr and tracks exit code. Passed through the execution, not mutated from lambdas.

### Consumer composability via R3 extensions

Add C#14 `extension(IFfmpegSession)` block with R3 operators like `PercentageProgress(duration)`. The reactivity lives at the consumer boundary, not inside the session.
