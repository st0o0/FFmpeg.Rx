using System.Globalization;
using FFmpeg.Rx.Filters;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Builder;

public sealed class OutputOptions
{
    internal List<StreamMapping> Mappings { get; } = [];
    internal List<string> Arguments { get; } = [];
    internal bool SuppressOverwrite { get; private set; }
    internal IReadOnlyList<IVideoFilter>? VideoFilters { get; private set; }
    internal IReadOnlyList<IAudioFilter>? AudioFilters { get; private set; }
    internal string? FilterComplexString { get; private set; }
    internal List<string> LabelMappings { get; } = [];

    public OutputOptions Map(int inputIndex, int streamIndex, StreamType type, string codec)
    {
        Mappings.Add(new StreamMapping(inputIndex, streamIndex, type, codec));
        return this;
    }

    public OutputOptions Arg(string argument)
    {
        Arguments.Add(argument);
        return this;
    }

    public OutputOptions Metadata(string streamSpec, string value)
    {
        Arguments.Add($"-metadata:{streamSpec} {value}");
        return this;
    }

    public OutputOptions Crf(int value)
    {
        Arguments.Add($"-crf {value}");
        return this;
    }

    public OutputOptions VideoBitrate(string rate)
    {
        Arguments.Add($"-b:v {rate}");
        return this;
    }

    public OutputOptions AudioBitrate(string rate)
    {
        Arguments.Add($"-b:a {rate}");
        return this;
    }

    public OutputOptions Framerate(double fps)
    {
        Arguments.Add(string.Create(CultureInfo.InvariantCulture, $"-r {fps}"));
        return this;
    }

    public OutputOptions Framerate(string rational)
    {
        Arguments.Add($"-r {rational}");
        return this;
    }

    public OutputOptions Preset(string preset)
    {
        Arguments.Add($"-preset {preset}");
        return this;
    }

    public OutputOptions Profile(string profile)
    {
        Arguments.Add($"-profile:v {profile}");
        return this;
    }

    public OutputOptions PixelFormat(string format)
    {
        Arguments.Add($"-pix_fmt {format}");
        return this;
    }

    public OutputOptions ForceFormat(string format)
    {
        Arguments.Add($"-f {format}");
        return this;
    }

    public OutputOptions NoOverwrite()
    {
        SuppressOverwrite = true;
        return this;
    }

    public OutputOptions Tune(string tune)
    {
        Arguments.Add($"-tune {tune}");
        return this;
    }

    public OutputOptions Seek(TimeSpan position)
    {
        Arguments.Add($"-ss {FormatTime(position)}");
        return this;
    }

    public OutputOptions Duration(TimeSpan length)
    {
        Arguments.Add(string.Create(CultureInfo.InvariantCulture, $"-t {length.TotalSeconds:F3}"));
        return this;
    }

    public OutputOptions StopTime(TimeSpan position)
    {
        Arguments.Add($"-to {FormatTime(position)}");
        return this;
    }

    public OutputOptions VideoSpeed(double factor)
    {
        var ptsFactor = 1.0 / factor;
        Arguments.Add(string.Create(CultureInfo.InvariantCulture, $"-filter:v \"setpts={ptsFactor:G}*PTS\""));
        return this;
    }

    public OutputOptions AudioTempo(double factor)
    {
        var chain = BuildAtempoChain(factor);
        Arguments.Add($"-filter:a \"{chain}\"");
        return this;
    }

    private static string FormatTime(TimeSpan ts)
    {
        return ts.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
    }

    public OutputOptions VideoFilter(Action<VideoFilterBuilder> configure)
    {
        var builder = new VideoFilterBuilder();
        configure(builder);
        VideoFilters = builder.Filters;
        return this;
    }

    public OutputOptions AudioFilter(Action<AudioFilterBuilder> configure)
    {
        var builder = new AudioFilterBuilder();
        configure(builder);
        AudioFilters = builder.Filters;
        return this;
    }

    public OutputOptions FilterComplex(FilterGraph.FilterGraph graph)
    {
        FilterComplexString = graph.Serialize();
        return this;
    }

    public OutputOptions MapLabel(string label)
    {
        LabelMappings.Add(label);
        return this;
    }

    // Phase 10: Metadata & Chapters
    public OutputOptions StreamMetadata(string key, string value, StreamType type, int? index = null)
    {
        var spec = index.HasValue
            ? $"s:{Internal.ArgumentBuilder.StreamTypeChar(type)}:{index}"
            : $"s:{Internal.ArgumentBuilder.StreamTypeChar(type)}";
        Arguments.Add($"-metadata:{spec} {key}={value}");
        return this;
    }

    public OutputOptions GlobalMetadata(string key, string value)
    {
        Arguments.Add($"-metadata {key}={value}");
        return this;
    }

    public OutputOptions WithoutMetadata()
    {
        Arguments.Add("-map_metadata -1");
        return this;
    }

    public OutputOptions CopyMetadataFrom(int inputIndex)
    {
        Arguments.Add($"-map_metadata {inputIndex}");
        return this;
    }

    public OutputOptions Disposition(StreamType type, int index, params string[] flags)
    {
        var flagStr = string.Join("+", flags);
        Arguments.Add($"-disposition:{Internal.ArgumentBuilder.StreamTypeChar(type)}:{index} {flagStr}");
        return this;
    }

    // Phase 14: Ecosystem Extras
    public OutputOptions NegativeMap(int inputIndex, StreamType type)
    {
        Arguments.Add($"-map -{inputIndex}:{Internal.ArgumentBuilder.StreamTypeChar(type)}");
        return this;
    }

    public OutputOptions CodecTag(string tag, StreamType type)
    {
        Arguments.Add($"-tag:{Internal.ArgumentBuilder.StreamTypeChar(type)} {tag}");
        return this;
    }

    public OutputOptions MovFlags(params string[] flags)
    {
        Arguments.Add($"-movflags {string.Join("+", flags)}");
        return this;
    }

    public OutputOptions MapAll(int inputIndex)
    {
        Arguments.Add($"-map {inputIndex}");
        return this;
    }

    public OutputOptions CopyStreams()
    {
        Arguments.Add("-c copy");
        return this;
    }

    private static string BuildAtempoChain(double factor)
    {
        var parts = new List<string>();
        var remaining = factor;

        while (remaining > 2.0)
        {
            parts.Add("atempo=2.0");
            remaining /= 2.0;
        }

        while (remaining < 0.5)
        {
            parts.Add("atempo=0.5");
            remaining /= 0.5;
        }

        parts.Add(string.Create(CultureInfo.InvariantCulture, $"atempo={remaining:G}"));
        return string.Join(",", parts);
    }
}
