## 1. Progress stderr fallback

- [x] 1.1 In `ArgumentSerializer`, detect PipeDestination and rewrite `-progress pipe:1` to `-progress pipe:2`
- [x] 1.2 In `FfmpegSession.ExecuteWithPipeOutput`, parse stderr lines through ProgressParser before emitting LogEvent
- [x] 1.3 Unit test: serializer emits `-progress pipe:2` when PipeDestination is present
- [x] 1.4 Unit test: serializer keeps `-progress pipe:1` when no PipeDestination

## 2. Integration tests: Tee and Multi-Output

- [x] 2.1 `TeeIntegrationTests.Tee_split_two_files` - two targets, both valid
- [x] 2.2 `MultiOutputIntegrationTests.Two_outputs_different_quality` - CRF 18 vs 28, size differs
- [x] 2.3 `NullOutputIntegrationTests.Null_output_completes` - `-f null` with audio filter
- [x] 2.4 `FluentApiIntegrationTests.Fluent_transcode_end_to_end` - Ffmpeg.Input().Output() with real encode

## 3. Integration tests: Pipe I/O

- [x] 3.1 `PipeInputIntegrationTests.Pipe_raw_audio_input` - generate raw PCM, pipe as stdin, encode to file
- [x] 3.2 `PipeOutputIntegrationTests.Pipe_output_to_stream` - decode file to raw stdout, capture in MemoryStream
- [x] 3.3 `PipeProgressIntegrationTests.Pipe_output_with_progress` - verify progress events arrive via stderr

## 4. Finalize

- [x] 4.1 All unit tests pass
- [x] 4.2 All integration tests pass
- [x] 4.3 `dotnet build` - zero errors, zero warnings
- [x] 4.4 `dotnet format --verify-no-changes` - clean
