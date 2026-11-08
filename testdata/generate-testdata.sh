#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

echo "Generating synthetic test media using ffmpeg lavfi..."

# video_h264_aac_5s.mp4 — 320x240 H264 + AAC, 5 seconds
ffmpeg -y -f lavfi -i testsrc=duration=5:size=320x240:rate=30 \
       -f lavfi -i sine=frequency=440:duration=5 \
       -c:v libx264 -crf 28 -preset ultrafast \
       -c:a aac -b:a 64k \
       video_h264_aac_5s.mp4

# video_only_3s.mp4 — 320x240 H264, no audio, 3 seconds
ffmpeg -y -f lavfi -i testsrc=duration=3:size=320x240:rate=30 \
       -c:v libx264 -crf 28 -preset ultrafast \
       -an \
       video_only_3s.mp4

# audio_only_3s.m4a — AAC, 3 seconds
ffmpeg -y -f lavfi -i sine=frequency=440:duration=3 \
       -c:a aac -b:a 64k \
       audio_only_3s.m4a

# video_hevc_3s.mp4 — 320x240 HEVC, 3 seconds
ffmpeg -y -f lavfi -i testsrc=duration=3:size=320x240:rate=30 \
       -c:v libx265 -crf 28 -preset ultrafast \
       -an \
       video_hevc_3s.mp4

# two_audio_tracks.mkv — H264 + 2 audio tracks (440Hz + 880Hz), 3 seconds
ffmpeg -y -f lavfi -i testsrc=duration=3:size=320x240:rate=30 \
       -f lavfi -i sine=frequency=440:duration=3 \
       -f lavfi -i sine=frequency=880:duration=3 \
       -map 0:v -map 1:a -map 2:a \
       -c:v libx264 -crf 28 -preset ultrafast \
       -c:a aac -b:a 64k \
       two_audio_tracks.mkv

# corrupt.mp4 — first 512 bytes of the video file
head -c 512 video_h264_aac_5s.mp4 > corrupt.mp4

echo "Done. Generated files:"
ls -lh *.mp4 *.m4a *.mkv *.srt 2>/dev/null || true
