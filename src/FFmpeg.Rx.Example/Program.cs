using FFmpeg.Rx;
using FFmpeg.Rx.Codecs;

// Build a transcode pipeline (does not execute ffmpeg)
await using var session = Ffmpeg.Input("input.mp4")
    .WithProgressReporting()
    .Output("output.mp4", o => o
        .Stream(0, 0, StreamType.Video, s => s
            .Encode(VideoCodec.H264, e => e
                .Quality(Quality.Crf(23))
                .Preset(Preset.Slow)))
        .Stream(0, 0, StreamType.Audio, s => s
            .Encode(AudioCodec.Aac, e => e
                .Bitrate(Bitrate.Kbps(192)))));

Console.WriteLine("Pipeline built successfully.");

// Build a remux pipeline
await using var remux = Ffmpeg.Input("input.mp4")
    .Output("output.mkv", o => o
        .Stream(0, 0, StreamType.Video, s => s.Copy())
        .Stream(0, 0, StreamType.Audio, s => s.Copy()));

Console.WriteLine("Remux pipeline built successfully.");

// Verify probing types are AOT-compatible
var info = new FFmpeg.Rx.Probing.MediaInfo(
    TimeSpan.FromSeconds(120),
    5_000_000,
    "mp4",
    [],
    [],
    [],
    [],
    "{}");

Console.WriteLine($"MediaInfo: {info.Duration}, {info.FormatName}");
Console.WriteLine("All types resolved successfully under AOT.");
