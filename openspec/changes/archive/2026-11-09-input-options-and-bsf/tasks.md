## 1. InputOptionConfig — add HTTP properties

- [x] 1.1 Add `UserAgent` (`string?`), `HttpProxy` (`string?`), `Headers` (`string?`) parameters to `InputOptionConfig` record in `Models/InputConfig.cs`
- [x] 1.2 Add Layer 1 tests: construct `InputOptionConfig` with HTTP properties, assert values

## 2. InputOptionsBuilder — new builder type

- [x] 2.1 Create `Builder/InputOptionsBuilder.cs` with fluent methods for all `InputOptionConfig` properties (Seek, HWAccel, HWAccelDevice, HWAccelOutputFormat, Decoder, Framerate, Loop, StreamLoop, UserAgent, HttpProxy, Headers, CustomArg) and internal `Build()` returning `InputOptionConfig`
- [x] 2.2 Add Layer 2 tests: exercise each builder method, assert built `InputOptionConfig` matches expected

## 3. PipelineBuilder — Input overloads and AddInput visibility

- [x] 3.1 Add `Input(Uri, Action<InputOptionsBuilder>)` overload on `PipelineBuilder`
- [x] 3.2 Add `Input(string, Action<InputOptionsBuilder>)` overload on `PipelineBuilder`
- [x] 3.3 Add `Input(Stream, string, Action<InputOptionsBuilder>)` overload on `PipelineBuilder`
- [x] 3.4 Change `AddInput(InputConfig)` from `internal` to `public`
- [x] 3.5 Add Layer 2 tests: builder with configured input options produces correct `InputConfig` records

## 4. Ffmpeg static entry — matching overloads

- [x] 4.1 Add `Ffmpeg.Input(Uri, Action<InputOptionsBuilder>)` static overload
- [x] 4.2 Add `Ffmpeg.Input(string, Action<InputOptionsBuilder>)` static overload
- [x] 4.3 Add `Ffmpeg.Input(Stream, string, Action<InputOptionsBuilder>)` static overload

## 5. ArgumentSerializer — HTTP input option serialization

- [x] 5.1 Serialize `UserAgent` as `-user_agent "<value>"` before `-i`
- [x] 5.2 Serialize `HttpProxy` as `-http_proxy <value>` before `-i`
- [x] 5.3 Serialize `Headers` as `-headers "<value>"` before `-i`
- [x] 5.4 Add Layer 3 tests: known `InputOptionConfig` with HTTP props → assert CLI string contains correct flags in correct position

## 6. StreamConfig — BitstreamFilter property

- [x] 6.1 Add `BitstreamFilter` (`string?`, default `null`) parameter to `StreamConfig` record
- [x] 6.2 Add Layer 1 tests: construct `StreamConfig` with BSF, assert value

## 7. StreamBuilder — BitstreamFilter fluent method

- [x] 7.1 Add `.BitstreamFilter(string)` method to `StreamBuilder`, wire to `StreamConfig.BitstreamFilter`
- [x] 7.2 Add Layer 2 tests: builder with BSF produces correct `StreamConfig`

## 8. ArgumentSerializer — BSF serialization

- [x] 8.1 Serialize `StreamConfig.BitstreamFilter` as `-bsf:<type> <filter>` based on `StreamType`
- [x] 8.2 Add Layer 3 tests: streams with BSF → assert `-bsf:a`, `-bsf:v`, `-bsf:s` in output
