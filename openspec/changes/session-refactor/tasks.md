## 1. Extract pure functions

- [ ] 1.1 Create `EmitProgress(ProgressEvent, TimeSpan?)` returning `IEnumerable<FfmpegEvent>`
- [ ] 1.2 Create `ClassifyStderrLine(string, Dictionary, bool parseProgress)` returning `FfmpegEvent`
- [ ] 1.3 Create `ClassifyLogLevel(string)` (already exists, keep as static)
- [ ] 1.4 Extract `Cancelled(Stopwatch)` and `StartFailed(Stopwatch, Exception)` helper methods
- [ ] 1.5 Unit tests for EmitProgress (with and without duration)
- [ ] 1.6 Unit tests for ClassifyStderrLine (progress line vs log line)

## 2. Merge execution paths

- [ ] 2.1 Create `ExecutionMode` enum (EventStream, PipeOutput)
- [ ] 2.2 Determine mode in RunAsync from PipelineSpec
- [ ] 2.3 Create `SessionState` class (StderrLines, ProgressBlock, ExitCode)
- [ ] 2.4 Rewrite RunAsync to single execution flow using mode + state
- [ ] 2.5 Delete ExecuteWithEventStream and ExecuteWithPipeOutput
- [ ] 2.6 Verify all 350 tests still pass

## 3. R3 extension block

- [ ] 3.1 Create `Extensions.cs` with C#14 `extension(IFfmpegSession)` block
- [ ] 3.2 Add `PercentageProgress(TimeSpan)` operator returning `Observable<double>`
- [ ] 3.3 Verify C#14 extension syntax compiles on current SDK
- [ ] 3.4 Unit test for PercentageProgress extension

## 4. Finalize

- [ ] 4.1 All unit tests pass
- [ ] 4.2 All integration tests pass
- [ ] 4.3 `dotnet build` zero errors, zero warnings
- [ ] 4.4 `dotnet format --verify-no-changes` clean
