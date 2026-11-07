namespace FFmpeg.Rx.Filters;

public sealed record CustomFilter(string Name, string? Args = null) : IVideoFilter, IAudioFilter
{
    public string Render() => Args is null ? Name : $"{Name}={Args}";
}
