# integration-gaps Specification

## Purpose
TBD - created by archiving change pipe-io-and-integration. Update Purpose after archive.
## Requirements
### Requirement: Tee muxer integration test
A test SHALL verify that tee muxer produces multiple output files from a single ffmpeg process.

#### Scenario: Tee split to two files
- **WHEN** a tee output with two targets is executed
- **THEN** both output files exist and contain valid media

### Requirement: Multi-output integration test
A test SHALL verify that multiple outputs from a single process produce correct files.

#### Scenario: Two outputs different quality
- **WHEN** a multi-output session encodes to CRF 18 and CRF 28
- **THEN** both files exist and the CRF-28 file is smaller

### Requirement: Null output integration test
A test SHALL verify the analysis pattern (null output with filter detection).

#### Scenario: Null output with audio filter
- **WHEN** a session outputs to null with a silence detection filter
- **THEN** the session completes successfully

### Requirement: Fluent API end-to-end integration test
A test SHALL verify the full `Ffmpeg.Input().Output()` fluent path with real ffmpeg.

#### Scenario: Fluent transcode
- **WHEN** `Ffmpeg.Input(file).Output(path, o => o.Stream(...).Encode(...))` runs
- **THEN** the output file is a valid media file

