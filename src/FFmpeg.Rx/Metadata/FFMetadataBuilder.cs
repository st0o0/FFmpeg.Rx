using System.Globalization;
using System.Text;

namespace FFmpeg.Rx.Metadata;

public sealed class FFMetadataBuilder
{
    private string? _title;
    private readonly List<(string Title, TimeSpan Start, TimeSpan End)> _chapters = [];

    public FFMetadataBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public FFMetadataBuilder WithChapter(string title, TimeSpan start, TimeSpan end)
    {
        _chapters.Add((title, start, end));
        return this;
    }

    public FFMetadataBuilder WithChapter(string title, TimeSpan duration)
    {
        var start = _chapters.Count > 0 ? _chapters[^1].End : TimeSpan.Zero;
        _chapters.Add((title, start, start + duration));
        return this;
    }

    public string Build()
    {
        var sb = new StringBuilder();
        sb.AppendLine(";FFMETADATA1");

        if (_title is not null)
        {
            sb.Append("title=").AppendLine(_title);
        }

        foreach (var (title, start, end) in _chapters)
        {
            sb.AppendLine("[CHAPTER]");
            sb.AppendLine("TIMEBASE=1/1000");
            sb.Append("START=").AppendLine(((long)start.TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
            sb.Append("END=").AppendLine(((long)end.TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
            sb.Append("title=").AppendLine(title);
        }

        return sb.ToString();
    }
}
