using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FFmpeg.Rx.Probing;

namespace FFmpeg.Rx.Internal;

internal static class ProbeResultParser
{
    internal static MediaInfo Parse(string json)
    {
        var doc = JsonSerializer.Deserialize(json, ProbeJsonContext.Default.ProbeResult)
            ?? throw new InvalidOperationException("Failed to parse ffprobe output");

        var format = doc.Format;
        var duration = ParseDuration(format?.Duration);
        var bitrate = ParseLong(format?.BitRate);
        var formatName = format?.FormatName ?? "";

        var videoStreams = new List<VideoStreamInfo>();
        var audioStreams = new List<AudioStreamInfo>();
        var subtitleStreams = new List<SubtitleStreamInfo>();

        if (doc.Streams is not null)
        {
            foreach (var stream in doc.Streams)
            {
                switch (stream.CodecType)
                {
                    case "video":
                        videoStreams.Add(new VideoStreamInfo(
                            stream.Index,
                            stream.CodecName ?? "",
                            stream.Width ?? 0,
                            stream.Height ?? 0,
                            ParseFps(stream.RFrameRate),
                            ParseLong(stream.BitRate),
                            stream.PixFmt,
                            ParseDuration(stream.Duration)));
                        break;
                    case "audio":
                        audioStreams.Add(new AudioStreamInfo(
                            stream.Index,
                            stream.CodecName ?? "",
                            ParseInt(stream.SampleRate),
                            stream.Channels ?? 0,
                            ParseLong(stream.BitRate),
                            ParseDuration(stream.Duration)));
                        break;
                    case "subtitle":
                        subtitleStreams.Add(new SubtitleStreamInfo(
                            stream.Index,
                            stream.CodecName ?? "",
                            stream.Tags?.Language));
                        break;
                }
            }
        }

        var chapters = new List<ChapterInfo>();
        if (doc.Chapters is not null)
        {
            foreach (var ch in doc.Chapters)
            {
                chapters.Add(new ChapterInfo(
                    ch.Tags?.Title,
                    ParseDurationSeconds(ch.StartTime),
                    ParseDurationSeconds(ch.EndTime)));
            }
        }

        return new MediaInfo(duration, bitrate, formatName,
            videoStreams, audioStreams, subtitleStreams, chapters, json);
    }

    private static TimeSpan ParseDuration(string? value)
    {
        if (value is null)
        {
            return TimeSpan.Zero;
        }

        return double.TryParse(value, CultureInfo.InvariantCulture, out var seconds)
            ? TimeSpan.FromSeconds(seconds)
            : TimeSpan.Zero;
    }

    private static TimeSpan ParseDurationSeconds(string? value) => ParseDuration(value);

    private static long ParseLong(string? value) => value is not null && long.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : 0;

    private static int ParseInt(string? value) => value is not null && int.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : 0;

    private static double ParseFps(string? value)
    {
        if (value is null)
        {
            return 0;
        }

        var parts = value.Split('/');
        if (parts.Length == 2 &&
            double.TryParse(parts[0], CultureInfo.InvariantCulture, out var num) &&
            double.TryParse(parts[1], CultureInfo.InvariantCulture, out var den) &&
            den > 0)
        {
            return num / den;
        }
        return double.TryParse(value, CultureInfo.InvariantCulture, out var fps) ? fps : 0;
    }
}

internal sealed class ProbeResult
{
    [JsonPropertyName("format")]
    public ProbeFormat? Format { get; set; }

    [JsonPropertyName("streams")]
    public ProbeStream[]? Streams { get; set; }

    [JsonPropertyName("chapters")]
    public ProbeChapter[]? Chapters { get; set; }
}

internal sealed class ProbeFormat
{
    [JsonPropertyName("format_name")]
    public string? FormatName { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("bit_rate")]
    public string? BitRate { get; set; }
}

internal sealed class ProbeStream
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("codec_type")]
    public string? CodecType { get; set; }

    [JsonPropertyName("codec_name")]
    public string? CodecName { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("r_frame_rate")]
    public string? RFrameRate { get; set; }

    [JsonPropertyName("bit_rate")]
    public string? BitRate { get; set; }

    [JsonPropertyName("pix_fmt")]
    public string? PixFmt { get; set; }

    [JsonPropertyName("sample_rate")]
    public string? SampleRate { get; set; }

    [JsonPropertyName("channels")]
    public int? Channels { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("tags")]
    public StreamTags? Tags { get; set; }
}

internal sealed class StreamTags
{
    [JsonPropertyName("language")]
    public string? Language { get; set; }
}

internal sealed class ProbeChapter
{
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    [JsonPropertyName("tags")]
    public ChapterTags? Tags { get; set; }
}

internal sealed class ChapterTags
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

[JsonSerializable(typeof(ProbeResult))]
internal sealed partial class ProbeJsonContext : JsonSerializerContext;
