## ADDED Requirements

### Requirement: Record live stream to file
The system SHALL support recording from any ffmpeg protocol URL (RTMP, HLS, RTSP, SRT) to a file using stream copy.

#### Scenario: Record RTMP stream
- **WHEN** user calls `Ffmpeg.Input(new Uri("rtmp://live.example.com/stream")).Output("recording.ts", o => o.Map(0, 0, StreamType.Video, Codec.Copy).Map(0, 0, StreamType.Audio, Codec.Copy))`
- **THEN** ArgumentBuilder emits `-i rtmp://live.example.com/stream -map 0:v:0 -map 0:a:0 -c:v copy -c:a copy -y "recording.ts"`

### Requirement: Recording with time limit
Recording SHALL respect `.Duration(TimeSpan)` on output options to limit recording length.

#### Scenario: Record 60 seconds
- **WHEN** user adds `.Duration(TimeSpan.FromSeconds(60))` to output options while recording a stream
- **THEN** ffmpeg stops after 60 seconds of output

### Requirement: Container recommendation for long recordings
The library documentation SHALL recommend `.ts` or `.mkv` for long recordings since mp4 finalizes its index only at the end and a crash loses the file.

#### Scenario: User chooses mp4 for recording
- **WHEN** user records to an mp4 file
- **THEN** the system does not block it (user's choice) but the format works correctly when ffmpeg finalizes
