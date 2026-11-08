# Test Media

Synthetic test files generated with ffmpeg's `lavfi` virtual input device.
These are algorithmically generated patterns (color bars, sine waves) — no copyrighted content.

## License

**CC0 1.0 Universal (Public Domain).** These files are synthetic test patterns
with no creative content. Use freely without restriction.

## Files

| File | Content | Codec | Duration | Size |
|------|---------|-------|----------|------|
| `video_h264_aac_5s.mp4` | testsrc 320x240 + sine 440Hz | H264 + AAC | 5s | ~110KB |
| `video_only_3s.mp4` | testsrc 320x240 | H264 | 3s | ~44KB |
| `audio_only_3s.m4a` | sine 440Hz | AAC | 3s | ~26KB |
| `video_hevc_3s.mp4` | testsrc 320x240 | HEVC | 3s | ~28KB |
| `subtitle.srt` | 3 timed entries | SRT text | 5s | <1KB |
| `two_audio_tracks.mkv` | testsrc + sine 440Hz + sine 880Hz | H264 + 2xAAC | 3s | ~93KB |
| `corrupt.mp4` | First 512 bytes of video_h264_aac_5s.mp4 | — | — | 512B |

## Regeneration

Run `generate-testdata.sh` with ffmpeg installed to regenerate all files:

```bash
bash generate-testdata.sh
```

All files are generated from ffmpeg's built-in `testsrc` video source and `sine` audio source.
No external media files are used.
