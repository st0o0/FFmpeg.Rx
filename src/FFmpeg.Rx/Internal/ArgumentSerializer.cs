using System.Globalization;
using System.Text;
using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Internal;

internal static class ArgumentSerializer
{
    internal static string Serialize(PipelineSpec pipeline)
    {
        var sb = new StringBuilder();

        if (pipeline.GlobalOptions is { Count: > 0 })
        {
            foreach (var opt in pipeline.GlobalOptions)
            {
                sb.Append(opt).Append(' ');
            }
        }

        foreach (var input in pipeline.Inputs)
        {
            SerializeInput(sb, input);
        }

        foreach (var output in pipeline.Outputs)
        {
            SerializeOutput(sb, output);
        }

        return sb.ToString();
    }

    private static void SerializeInput(StringBuilder sb, InputSpec input)
    {
        if (input.Options is { } opts)
        {
            SerializeInputOptions(sb, opts);
        }

        switch (input)
        {
            case UrlInputSpec url:
                sb.Append("-i ").Append(url.Url).Append(' ');
                break;
            case FileInputSpec file:
                sb.Append("-i \"").Append(file.Path).Append("\" ");
                break;
            case PipeInputSpec pipe:
                if (pipe.Format is { Length: > 0 })
                {
                    sb.Append("-f ").Append(pipe.Format).Append(' ');
                }

                sb.Append("-i pipe:0 ");
                break;
        }
    }

    private static void SerializeInputOptions(StringBuilder sb, InputOptionSpec opts)
    {
        if (opts.HWAccel is { } hwaccel)
        {
            sb.Append("-hwaccel ").Append(hwaccel.Name).Append(' ');
        }

        if (opts.HWAccelDevice is { } device)
        {
            sb.Append("-hwaccel_device ").Append(device).Append(' ');
        }

        if (opts.HWAccelOutputFormat is { } format)
        {
            sb.Append("-hwaccel_output_format ").Append(format).Append(' ');
        }

        if (opts.VideoDecoder is { } decoder)
        {
            sb.Append("-c:v ").Append(decoder.Name).Append(' ');
        }

        if (opts.Framerate is { } fps)
        {
            sb.Append("-framerate ").Append(fps.ToFfmpegString()).Append(' ');
        }

        if (opts.Loop)
        {
            sb.Append("-loop 1 ");
        }

        if (opts.StreamLoop is { } streamLoop)
        {
            sb.Append("-stream_loop ").Append(streamLoop).Append(' ');
        }

        if (opts.Seek is { } seek)
        {
            sb.Append("-ss ").Append(FormatTime(seek)).Append(' ');
        }

        if (opts.CustomArgs is { Count: > 0 })
        {
            foreach (var arg in opts.CustomArgs)
            {
                sb.Append(arg).Append(' ');
            }
        }
    }

    private static void SerializeOutput(StringBuilder sb, OutputSpec output)
    {
        if (output.Filters is { } filters)
        {
            SerializeFilters(sb, filters);
        }

        if (output.Streams is { Count: > 0 })
        {
            SerializeStreamMappings(sb, output.Streams);
            SerializeCodecs(sb, output.Streams);
        }

        if (output.LabelMappings is { Count: > 0 })
        {
            foreach (var label in output.LabelMappings)
            {
                sb.Append("-map \"[").Append(label).Append("]\" ");
            }
        }

        if (output.Container is { } container)
        {
            sb.Append("-f ").Append(container.Name).Append(' ');
        }

        if (output.TimeRange is { } timeRange)
        {
            SerializeTimeRange(sb, timeRange);
        }

        if (output.Metadata is { } metadata)
        {
            SerializeMetadata(sb, metadata);
        }

        if (output.FastStart)
        {
            sb.Append("-movflags +faststart ");
        }

        if (output.MovFlags is { Count: > 0 })
        {
            sb.Append("-movflags ").Append(string.Join("+", output.MovFlags)).Append(' ');
        }

        if (output.CustomArgs is { Count: > 0 })
        {
            foreach (var arg in output.CustomArgs)
            {
                sb.Append(arg).Append(' ');
            }
        }

        if (output.Tee is { } tee)
        {
            SerializeTee(sb, tee);
        }
        else
        {
            sb.Append(output.Overwrite ? "-y" : "-n");
            switch (output.Destination)
            {
                case FileDestination file:
                    sb.Append(" \"").Append(file.Path).Append('"');
                    break;
                case NullDestination:
                    sb.Append(" /dev/null");
                    break;
                case PipeDestination:
                    sb.Append(" pipe:1");
                    break;
            }
        }
    }

    private static void SerializeStreamMappings(StringBuilder sb, IReadOnlyList<StreamSpec> streams)
    {
        foreach (var stream in streams)
        {
            sb.Append("-map ").Append(stream.InputIndex).Append(':').Append(StreamTypeChar(stream.Type));
            if (stream.StreamIndex.HasValue)
            {
                sb.Append(':').Append(stream.StreamIndex.Value);
            }

            sb.Append(' ');
        }
    }

    private static void SerializeCodecs(StringBuilder sb, IReadOnlyList<StreamSpec> streams)
    {
        var seen = new HashSet<StreamType>();
        foreach (var stream in streams)
        {
            if (!seen.Add(stream.Type))
            {
                continue;
            }

            SerializeCodec(sb, stream.Codec, stream.Type);
        }
    }

    private static void SerializeCodec(StringBuilder sb, CodecSpec codec, StreamType type)
    {
        switch (codec)
        {
            case StreamCopy:
                sb.Append("-c:").Append(StreamTypeChar(type)).Append(" copy ");
                break;
            case VideoEncode ve:
                sb.Append("-c:v ").Append(ve.Codec.Name).Append(' ');
                SerializeVideoEncoding(sb, ve.Options);
                break;
            case AudioEncode ae:
                sb.Append("-c:a ").Append(ae.Codec.Name).Append(' ');
                SerializeAudioEncoding(sb, ae.Options);
                break;
            case SubtitleEncode se:
                sb.Append("-c:s ").Append(se.Codec.Name).Append(' ');
                break;
        }
    }

    private static void SerializeVideoEncoding(StringBuilder sb, VideoEncodingOptions opts)
    {
        if (opts.Quality is { } quality)
        {
            switch (quality)
            {
                case ConstantQuality cq:
                    sb.Append("-crf ").Append(cq.Crf).Append(' ');
                    break;
                case ConstantBitrate cb:
                    sb.Append("-b:v ").Append(cb.Rate.ToFfmpegString()).Append(' ');
                    break;
            }
        }

        if (opts.Preset is { } preset)
        {
            sb.Append("-preset ").Append(preset.Name).Append(' ');
        }

        if (opts.Profile is { } profile)
        {
            sb.Append("-profile:v ").Append(profile.Name).Append(' ');
        }

        if (opts.Tune is { } tune)
        {
            sb.Append("-tune ").Append(tune.Name).Append(' ');
        }

        if (opts.PixelFormat is { } pixFmt)
        {
            sb.Append("-pix_fmt ").Append(pixFmt.Name).Append(' ');
        }

        if (opts.Framerate is { } fps)
        {
            sb.Append("-r ").Append(fps.ToFfmpegString()).Append(' ');
        }
    }

    private static void SerializeAudioEncoding(StringBuilder sb, AudioEncodingOptions opts)
    {
        if (opts.Bitrate is { } bitrate)
        {
            sb.Append("-b:a ").Append(bitrate.ToFfmpegString()).Append(' ');
        }

        if (opts.SampleRate is { } sampleRate)
        {
            sb.Append("-ar ").Append(sampleRate).Append(' ');
        }

        if (opts.Channels is { } channels)
        {
            sb.Append("-ac ").Append(channels).Append(' ');
        }
    }

    private static void SerializeTimeRange(StringBuilder sb, TimeRange range)
    {
        if (range.Seek is { } seek)
        {
            sb.Append("-ss ").Append(FormatTime(seek)).Append(' ');
        }

        if (range.Duration is { } duration)
        {
            sb.Append(string.Create(CultureInfo.InvariantCulture, $"-t {duration.TotalSeconds:F3}")).Append(' ');
        }

        if (range.StopTime is { } stopTime)
        {
            sb.Append("-to ").Append(FormatTime(stopTime)).Append(' ');
        }
    }

    private static void SerializeMetadata(StringBuilder sb, MetadataSpec metadata)
    {
        if (metadata.StripAll)
        {
            sb.Append("-map_metadata -1 ");
        }

        if (metadata.CopyFromInput is { } inputIndex)
        {
            sb.Append("-map_metadata ").Append(inputIndex).Append(' ');
        }

        if (metadata.GlobalEntries is { Count: > 0 })
        {
            foreach (var entry in metadata.GlobalEntries)
            {
                sb.Append("-metadata ").Append(entry.Key).Append("=\"").Append(entry.Value).Append("\" ");
            }
        }
    }

    private static void SerializeFilters(StringBuilder sb, FilterSpec filters)
    {
        switch (filters)
        {
            case SimpleFilterSpec simple:
                if (simple.VideoFilters is { Count: > 0 })
                {
                    sb.Append("-vf \"");
                    for (var i = 0; i < simple.VideoFilters.Count; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(',');
                        }

                        sb.Append(simple.VideoFilters[i].Render());
                    }

                    sb.Append("\" ");
                }

                if (simple.AudioFilters is { Count: > 0 })
                {
                    sb.Append("-af \"");
                    for (var i = 0; i < simple.AudioFilters.Count; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(',');
                        }

                        sb.Append(simple.AudioFilters[i].Render());
                    }

                    sb.Append("\" ");
                }

                break;
            case ComplexFilterSpec complex:
                sb.Append("-filter_complex \"").Append(complex.Graph.Serialize()).Append("\" ");
                break;
        }
    }

    private static void SerializeTee(StringBuilder sb, TeeOutput tee)
    {
        sb.Append("-f tee \"");
        for (var i = 0; i < tee.Targets.Length; i++)
        {
            if (i > 0)
            {
                sb.Append('|');
            }

            var target = tee.Targets[i];
            var opts = new List<string>();
            if (target.Format is not null)
            {
                opts.Add($"f={target.Format}");
            }

            if (target.Select is not null)
            {
                opts.Add($"select={target.Select}");
            }

            if (target.BitstreamFilter is not null)
            {
                opts.Add($"bsf={target.BitstreamFilter}");
            }

            if (target.MovFlags is not null)
            {
                opts.Add($"movflags={target.MovFlags}");
            }

            if (target.IgnoreFailure)
            {
                opts.Add("onfail=ignore");
            }

            if (opts.Count > 0)
            {
                sb.Append('[').Append(string.Join(":", opts)).Append(']');
            }

            sb.Append(target.Path);
        }

        sb.Append('"');
    }

    private static string FormatTime(TimeSpan ts) => ts.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);

    internal static char StreamTypeChar(StreamType type)
    {
        return type switch
        {
            StreamType.Video => 'v',
            StreamType.Audio => 'a',
            StreamType.Subtitle => 's',
            _ => '?'
        };
    }
}
