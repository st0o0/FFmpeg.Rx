namespace FFmpeg.Rx.Probing;

public sealed record MediaInfo(
    TimeSpan Duration,
    long Bitrate,
    string FormatName,
    IReadOnlyList<VideoStreamInfo> VideoStreams,
    IReadOnlyList<AudioStreamInfo> AudioStreams,
    IReadOnlyList<SubtitleStreamInfo> SubtitleStreams,
    IReadOnlyList<ChapterInfo> Chapters,
    string RawJson)
{
    public VideoStreamInfo? PrimaryVideo => VideoStreams.Count > 0 ? VideoStreams[0] : null;
    public AudioStreamInfo? PrimaryAudio => AudioStreams.Count > 0 ? AudioStreams[0] : null;
}

public sealed record VideoStreamInfo(
    int Index,
    string Codec,
    int Width,
    int Height,
    double Fps,
    long Bitrate,
    string? PixelFormat,
    TimeSpan Duration);

public sealed record AudioStreamInfo(
    int Index,
    string Codec,
    int SampleRate,
    int Channels,
    long Bitrate,
    TimeSpan Duration);

public sealed record SubtitleStreamInfo(
    int Index,
    string Codec,
    string? Language);

public sealed record ChapterInfo(
    string? Title,
    TimeSpan Start,
    TimeSpan End);
