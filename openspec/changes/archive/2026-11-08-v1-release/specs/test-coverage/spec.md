## ADDED Requirements

### Requirement: 100% builder coverage
Every public builder class SHALL have unit tests covering all methods. Each method test SHALL assert on the produced record's typed field value.

#### Scenario: VideoEncodingBuilder coverage
- **WHEN** `VideoEncodingBuilder` is tested
- **THEN** Quality, Preset, Profile, Tune, PixelFormat, Framerate methods are each tested with assertion on the built VideoEncodingOptions record

### Requirement: Fluent API end-to-end coverage
The `Ffmpeg.Input() → .Output() → session` fluent path SHALL be tested for all Output variants: Output, OutputPipe, OutputNull, OutputTee, Outputs.

#### Scenario: PipelineBuilder all output methods
- **WHEN** each output method is called
- **THEN** an IFfmpegSession is returned (and underlying PipelineSpec is correct)

### Requirement: Pipe I/O tests
Pipe input (Stream → ffmpeg) and pipe output (ffmpeg → Stream) SHALL be tested in integration tests.

#### Scenario: Pipe input integration
- **WHEN** a MemoryStream with raw video data is piped as input
- **THEN** ffmpeg processes it and produces output

### Requirement: Expression filter coverage
`CropExprFilter` and `VolumeExprFilter` SHALL have unit tests for their Render() output.

#### Scenario: CropExpr renders expression
- **WHEN** `CropExprFilter("iw/2", "ih/2", "(iw-ow)/2", "(ih-oh)/2").Render()` is called
- **THEN** it returns the correct ffmpeg filter string

### Requirement: Error path coverage
`FfprobeException`, `ErrorClassifier` edge cases, and `FfmpegSession` process-start failure SHALL be tested.

#### Scenario: FfprobeException on bad binary
- **WHEN** Ffprobe is called with a nonexistent binary path
- **THEN** FfprobeException is thrown

### Requirement: Tee builder coverage
`TeeBuilder` and `TeeTargetOptions` SHALL have unit tests verifying TeeOutput/TeeTarget record construction.

#### Scenario: TeeBuilder produces targets
- **WHEN** TeeBuilder is configured with two targets
- **THEN** the built TeeOutput has two TeeTarget records with correct properties
