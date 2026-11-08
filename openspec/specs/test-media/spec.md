# test-media Specification

## Purpose
TBD - created by archiving change integration-tests. Update Purpose after archive.
## Requirements
### Requirement: Synthetic test media in repository
The repository SHALL contain a `testdata/` directory with synthetic media files generated via ffmpeg's lavfi virtual input. Files SHALL be small (320x240, 3-5s) and committed directly (no LFS).

#### Scenario: Test media exists
- **WHEN** the repository is cloned
- **THEN** `testdata/` contains all required test files ready to use

### Requirement: License documentation
`testdata/README.md` SHALL document that files are synthetic (lavfi-generated), include the generation commands, and state a CC0 license.

#### Scenario: License clarity
- **WHEN** a developer inspects testdata/
- **THEN** README.md explains the origin and license of every file

### Requirement: Generation script
A `testdata/generate-testdata.sh` script SHALL reproduce all test files from scratch using only ffmpeg lavfi inputs.

#### Scenario: Reproducibility
- **WHEN** the script is run with ffmpeg installed
- **THEN** it regenerates all test files identically

### Requirement: Test file inventory
The following files SHALL exist:
- `video_h264_aac_5s.mp4` — video+audio, H264+AAC, 5 seconds
- `video_only_3s.mp4` — video only, H264, 3 seconds
- `audio_only_3s.m4a` — audio only, AAC, 3 seconds
- `video_hevc_3s.mp4` — video only, HEVC, 3 seconds
- `subtitle.srt` — SRT subtitle file with 3 timed entries
- `two_audio_tracks.mkv` — video + 2 audio streams, 3 seconds
- `corrupt.mp4` — intentionally truncated file for error tests

#### Scenario: Video test file has expected properties
- **WHEN** `video_h264_aac_5s.mp4` is probed
- **THEN** it has one H264 video stream (320x240) and one AAC audio stream, duration ~5s

