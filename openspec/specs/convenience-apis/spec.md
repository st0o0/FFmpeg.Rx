# convenience-apis Specification

## Purpose
Convenience APIs for common FFmpeg operations — snapshot extraction, file concatenation, and probe shortcuts.
## Requirements
### Requirement: Snapshot extraction
`Ffmpeg.Snapshot(string inputPath, TimeSpan time, string outputPath)` SHALL extract a single frame as an image file.

#### Scenario: Extract thumbnail at 5 seconds
- **WHEN** `await Ffmpeg.Snapshot("video.mp4", TimeSpan.FromSeconds(5), "thumb.jpg")` is called
- **THEN** `thumb.jpg` contains the frame at 5 seconds

### Requirement: Concat via demuxer
`Ffmpeg.Concat(IEnumerable<string> inputPaths, string outputPath)` SHALL concatenate files using the concat demuxer with stream copy.

#### Scenario: Join two MP4 files
- **WHEN** `await Ffmpeg.Concat(["part1.mp4", "part2.mp4"], "full.mp4")` is called
- **THEN** `full.mp4` contains both parts in order

#### Scenario: Temp file cleanup
- **WHEN** concat completes (success or failure)
- **THEN** the temporary concat list file is deleted

### Requirement: Probe shortcut
`Ffmpeg.Probe(string path)` SHALL be a shortcut for `Ffprobe.AnalyseAsync(path)`.

#### Scenario: Quick probe
- **WHEN** `var info = await Ffmpeg.Probe("video.mp4")` is called
- **THEN** it returns the same MediaInfo as `Ffprobe.AnalyseAsync`

