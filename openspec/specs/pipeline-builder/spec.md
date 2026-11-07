## ADDED Requirements

### Requirement: Fluent entry point
The static class `Ffmpeg` SHALL provide entry-point methods for building pipelines. `Ffmpeg.Input(Uri url)` and `Ffmpeg.Input(string filePath)` SHALL return a builder that accepts further inputs and an output. `Ffmpeg.At(string binaryPath)` SHALL return a builder that uses the specified FFmpeg binary instead of PATH lookup.

#### Scenario: URL input
- **WHEN** `Ffmpeg.Input(new Uri("https://example.com/video.mp4"))` is called
- **THEN** it SHALL return a builder with one URL input configured

#### Scenario: File input
- **WHEN** `Ffmpeg.Input("/path/to/file.srt")` is called
- **THEN** it SHALL return a builder with one file input configured

#### Scenario: Custom binary path
- **WHEN** `Ffmpeg.At("/usr/local/bin/ffmpeg").Input(url)` is called
- **THEN** the built session SHALL use `/usr/local/bin/ffmpeg` as the executable

### Requirement: Input options
URL inputs SHALL support option configuration via an `Action<InputOptions>` parameter. Options SHALL include `.UserAgent(string)`, `.Proxy(string)`, and `.Arg(string)` for custom arguments.

#### Scenario: URL with proxy and user agent
- **WHEN** `Ffmpeg.Input(url, i => i.UserAgent("Mozilla/5.0").Proxy("http://proxy:8888"))` is called
- **THEN** the generated arguments SHALL include `-user_agent "Mozilla/5.0"` and `-http_proxy http://proxy:8888` before the input URL

### Requirement: Multiple inputs
The builder SHALL support chaining `.Input(...)` calls to add multiple inputs. Each input SHALL be assigned a 0-based index used in stream mappings.

#### Scenario: Video URL plus subtitle file
- **WHEN** `.Input(videoUrl).Input(subtitlePath)` is chained
- **THEN** the pipeline SHALL have two inputs: index 0 (video URL) and index 1 (subtitle file)

### Requirement: Output configuration
`.Output(string path, Action<OutputOptions>)` SHALL configure the single output. Output options SHALL include `.Map(int inputIndex, int streamIndex, StreamType type, string codec)`, `.Arg(string)`, and `.Metadata(string streamSpec, string value)`.

#### Scenario: Remux with stream mapping
- **WHEN** `.Output("out.mkv", o => o.Map(0, 0, StreamType.Video, Codec.Copy).Map(0, 0, StreamType.Audio, Codec.Copy))` is configured
- **THEN** the generated arguments SHALL include `-map 0:v:0 -map 0:a:0 -c:v copy -c:a copy`

#### Scenario: Custom metadata
- **WHEN** `.Metadata("s:s:0", "language=deu")` is called on output options
- **THEN** the generated arguments SHALL include `-metadata:s:s:0 language=deu`

### Requirement: Progress reporting opt-in
`.WithProgressReporting()` on the builder SHALL add `-progress pipe:1` to the global arguments. Without it, no progress events are emitted.

#### Scenario: Progress enabled
- **WHEN** `.WithProgressReporting()` is called before `.Build()`
- **THEN** the FFmpeg arguments SHALL include `-progress pipe:1`

### Requirement: Build produces immutable pipeline and session
`.Build()` SHALL produce an `FfmpegSession` backed by an immutable `FfmpegPipeline` record. The pipeline record SHALL be constructible directly (without the builder) for testing purposes.

#### Scenario: Record construction without builder
- **WHEN** an `FfmpegPipeline` record is constructed directly with inputs, output, and options
- **THEN** it SHALL be usable to create an `FfmpegSession` via `FfmpegSession.Create(pipeline)`

### Requirement: Pipeline records are immutable
`FfmpegPipeline`, `FfmpegInput` (and subtypes `UrlInput`, `FileInput`), `FfmpegOutput`, and `StreamMapping` SHALL be immutable records. Mutation SHALL only be possible via `with` expressions producing new instances.

#### Scenario: Record immutability
- **WHEN** a pipeline is built
- **THEN** none of its properties SHALL be settable after construction
