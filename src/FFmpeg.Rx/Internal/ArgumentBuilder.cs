using System.Text;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Internal;

internal static class ArgumentBuilder
{
    internal static string Build(FfmpegPipeline pipeline)
    {
        var sb = new StringBuilder();

        foreach (var opt in pipeline.GlobalOptions)
        {
            sb.Append(opt).Append(' ');
        }

        foreach (var input in pipeline.Inputs)
        {
            AppendInput(sb, input);
        }

        if (pipeline.TeeOutput is not null)
        {
            AppendTee(sb, pipeline);
            return sb.ToString();
        }

        foreach (var output in pipeline.Outputs)
        {
            AppendOutput(sb, output);
        }

        return sb.ToString();
    }

    private static void AppendOutput(StringBuilder sb, FfmpegOutput output)
    {
        if (output.FilterComplexString is not null)
        {
            sb.Append("-filter_complex \"").Append(output.FilterComplexString).Append("\" ");
        }

        AppendFilters(sb, output);
        AppendStreamMappings(sb, output.Streams);
        AppendLabelMappings(sb, output.LabelMappings);
        AppendCodecs(sb, output.Streams);

        foreach (var arg in output.CustomArguments)
        {
            sb.Append(arg).Append(' ');
        }

        sb.Append(output.NoOverwrite ? "-n" : "-y").Append(" \"").Append(output.Path).Append('"');
    }

    private static void AppendTee(StringBuilder sb, FfmpegPipeline pipeline)
    {
        var output = pipeline.Output;
        AppendFilters(sb, output);
        AppendStreamMappings(sb, output.Streams);
        AppendLabelMappings(sb, output.LabelMappings);
        AppendCodecs(sb, output.Streams);

        foreach (var arg in output.CustomArguments)
        {
            sb.Append(arg).Append(' ');
        }

        sb.Append("-f tee \"");
        var tee = pipeline.TeeOutput!;
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

    private static void AppendInput(StringBuilder sb, FfmpegInput input)
    {
        foreach (var arg in input.CustomArguments)
        {
            sb.Append(arg).Append(' ');
        }

        switch (input)
        {
            case UrlInput url:
                if (url.UserAgent is not null)
                {
                    sb.Append("-user_agent \"").Append(url.UserAgent).Append("\" ");
                }

                if (url.Proxy is not null)
                {
                    sb.Append("-http_proxy ").Append(url.Proxy).Append(' ');
                }

                sb.Append("-i ").Append(url.Url).Append(' ');
                break;
            case FileInput file:
                sb.Append("-i \"").Append(file.Path).Append("\" ");
                break;
            case PipeInput pipe:
                if (pipe.Source.StreamFormat is { Length: > 0 })
                {
                    sb.Append("-f ").Append(pipe.Source.StreamFormat).Append(' ');
                }
                if (OperatingSystem.IsWindows())
                {
                    sb.Append(@"-i \\.\pipe\").Append(pipe.PipeName).Append(' ');
                }
                else
                {
                    sb.Append("-i pipe:").Append(pipe.PipeName).Append(' ');
                }
                break;
        }
    }

    private static void AppendFilters(StringBuilder sb, FfmpegOutput output)
    {
        if (output.VideoFilters is { Count: > 0 })
        {
            sb.Append("-vf \"");
            for (var i = 0; i < output.VideoFilters.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.Append(output.VideoFilters[i].Render());
            }
            sb.Append("\" ");
        }

        if (output.AudioFilters is { Count: > 0 })
        {
            sb.Append("-af \"");
            for (var i = 0; i < output.AudioFilters.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.Append(output.AudioFilters[i].Render());
            }
            sb.Append("\" ");
        }
    }

    private static void AppendStreamMappings(StringBuilder sb, StreamMapping[] streams)
    {
        foreach (var mapping in streams)
        {
            sb.Append("-map ")
                .Append(mapping.InputIndex)
                .Append(':')
                .Append(StreamTypeChar(mapping.Type))
                .Append(':')
                .Append(mapping.StreamIndex)
                .Append(' ');
        }
    }

    private static void AppendLabelMappings(StringBuilder sb, string[]? labels)
    {
        if (labels is null)
        {
            return;
        }

        foreach (var label in labels)
        {
            sb.Append("-map \"[").Append(label).Append("]\" ");
        }
    }

    private static void AppendCodecs(StringBuilder sb, StreamMapping[] streams)
    {
        var seen = new HashSet<StreamType>();
        foreach (var mapping in streams)
        {
            if (seen.Add(mapping.Type))
            {
                sb.Append("-c:")
                    .Append(StreamTypeChar(mapping.Type))
                    .Append(' ')
                    .Append(mapping.Codec)
                    .Append(' ');
            }
        }
    }

    internal static char StreamTypeChar(StreamType type) => type switch
    {
        StreamType.Video => 'v',
        StreamType.Audio => 'a',
        StreamType.Subtitle => 's',
        _ => '?'
    };
}
