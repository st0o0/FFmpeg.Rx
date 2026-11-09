# input-options-builder Specification

## Purpose
TBD - created by archiving change input-options-and-bsf. Update Purpose after archive.
## Requirements
### Requirement: InputOptionsBuilder fluent API
`InputOptionsBuilder` SHALL provide fluent methods for all `InputOptionConfig` properties. Each method SHALL return `this` for chaining. `Build()` SHALL return an `InputOptionConfig` record.

#### Scenario: Seek and HW accel
- **WHEN** `new InputOptionsBuilder().Seek(TimeSpan.FromSeconds(30)).HWAccel(HWAccel.Cuda).Build()` is called
- **THEN** it SHALL return `InputOptionConfig` with `Seek = 30s` and `HWAccel = HWAccel.Cuda`

#### Scenario: HTTP options
- **WHEN** `.UserAgent("Mozilla/5.0").HttpProxy("http://proxy:8888").Headers("Referer: https://example.com")` is called
- **THEN** the built `InputOptionConfig` SHALL have matching `UserAgent`, `HttpProxy`, and `Headers` values

#### Scenario: Custom args
- **WHEN** `.CustomArg("-reconnect 1").CustomArg("-reconnect_streamed 1")` is called
- **THEN** the built config SHALL have both args in `CustomArgs` in order

### Requirement: HTTP input properties on InputOptionConfig
`InputOptionConfig` SHALL include `UserAgent` (`string?`), `HttpProxy` (`string?`), and `Headers` (`string?`) as optional constructor parameters alongside existing properties.

#### Scenario: Direct record construction with HTTP options
- **WHEN** `new InputOptionConfig(UserAgent: "Mozilla/5.0", HttpProxy: "http://proxy:8888")` is constructed
- **THEN** it SHALL be a valid config with those properties set

### Requirement: PipelineBuilder Input overloads with configure action
`PipelineBuilder` SHALL provide `Input(Uri, Action<InputOptionsBuilder>)`, `Input(string, Action<InputOptionsBuilder>)`, and `Input(Stream, string, Action<InputOptionsBuilder>)` overloads. Each SHALL create the matching `InputConfig` subtype with `Options` set to the built `InputOptionConfig`.

#### Scenario: URL input with user-agent via builder
- **WHEN** `builder.Input(new Uri("https://example.com/video.mp4"), o => o.UserAgent("Mozilla/5.0"))` is called
- **THEN** the pipeline's input SHALL be a `UrlInputConfig` with `Options.UserAgent == "Mozilla/5.0"`

#### Scenario: File input with seek via builder
- **WHEN** `builder.Input("/path/to/file.mp4", o => o.Seek(TimeSpan.FromMinutes(1)))` is called
- **THEN** the pipeline's input SHALL be a `FileInputConfig` with `Options.Seek == 1 minute`

### Requirement: Ffmpeg static entry overloads with configure action
`Ffmpeg.Input(Uri, Action<InputOptionsBuilder>)`, `Ffmpeg.Input(string, Action<InputOptionsBuilder>)`, and `Ffmpeg.Input(Stream, string, Action<InputOptionsBuilder>)` SHALL exist and return `PipelineBuilder` with the configured input.

#### Scenario: Ffmpeg entry with proxy
- **WHEN** `Ffmpeg.Input(new Uri(url), o => o.HttpProxy("http://proxy:8888"))` is called
- **THEN** it SHALL return a `PipelineBuilder` with one `UrlInputConfig` having `Options.HttpProxy` set

### Requirement: AddInput made public
`PipelineBuilder.AddInput(InputConfig)` SHALL be `public`, allowing users to pass pre-constructed `InputConfig` records directly.

#### Scenario: Pre-built config via AddInput
- **WHEN** `builder.AddInput(new UrlInputConfig(uri) { Options = new InputOptionConfig(UserAgent: "Bot") })` is called
- **THEN** the pipeline SHALL include that input with the specified options

### Requirement: HTTP input option serialization
The `ArgumentSerializer` SHALL serialize `UserAgent` as `-user_agent "<value>"`, `HttpProxy` as `-http_proxy <value>`, and `Headers` as `-headers "<value>"` in the input option position (before `-i`).

#### Scenario: URL with user-agent and proxy serialized
- **GIVEN** a `UrlInputConfig` with `Options = new InputOptionConfig(UserAgent: "Mozilla/5.0", HttpProxy: "http://proxy:8888")`
- **WHEN** the pipeline is serialized
- **THEN** the output SHALL contain `-user_agent "Mozilla/5.0" -http_proxy http://proxy:8888` before `-i https://...`

