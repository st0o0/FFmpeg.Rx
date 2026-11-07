## ADDED Requirements

### Requirement: Hardware acceleration enum
The system SHALL provide an `HWAccel` static class with constants: Cuda, Qsv, Dxva2, D3d11va, VideoToolbox, Vaapi, Auto.

#### Scenario: Reference HW accel
- **WHEN** user references `HWAccel.Cuda`
- **THEN** its value is `"cuda"`

### Requirement: HW acceleration on input
`InputOptions` SHALL provide `.HWAccel(string accel)` and `.HWAccelDevice(string device)`.

#### Scenario: CUDA decode
- **WHEN** user calls `.HWAccel(HWAccel.Cuda)` on input options
- **THEN** ArgumentBuilder emits `-hwaccel cuda` before `-i`

#### Scenario: Specific GPU device
- **WHEN** user calls `.HWAccel(HWAccel.Cuda).HWAccelDevice("0")` on input options
- **THEN** ArgumentBuilder emits `-hwaccel cuda -hwaccel_device 0` before `-i`

### Requirement: HW decoder selection
`InputOptions` SHALL provide `.VideoDecoder(string decoder)` for hardware decoder selection.

#### Scenario: NVDEC decoder
- **WHEN** user calls `.VideoDecoder("h264_cuvid")` on input options
- **THEN** ArgumentBuilder emits `-c:v h264_cuvid` before `-i`

### Requirement: HW encoder selection
HW encoders SHALL be selectable via the existing codec system using constants like `VideoCodec.H264Nvenc`, `VideoCodec.HevcNvenc`, `VideoCodec.H264Qsv`, etc.

#### Scenario: NVENC encoding
- **WHEN** user maps video with `VideoCodec.H264Nvenc`
- **THEN** ArgumentBuilder emits `-c:v h264_nvenc`

### Requirement: HW accel output format
`InputOptions` SHALL provide `.HWAccelOutputFormat(string format)` to keep frames on GPU.

#### Scenario: Keep CUDA frames
- **WHEN** user calls `.HWAccelOutputFormat("cuda")` on input options
- **THEN** ArgumentBuilder emits `-hwaccel_output_format cuda` before `-i`
