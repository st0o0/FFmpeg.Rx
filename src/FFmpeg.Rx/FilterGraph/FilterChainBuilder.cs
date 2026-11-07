using FFmpeg.Rx.Filters;

namespace FFmpeg.Rx.FilterGraph;

public sealed class FilterChainBuilder
{
    private readonly List<IVideoFilter> _filters = [];

    public FilterChainBuilder Scale(int width, int height)
    {
        _filters.Add(new ScaleFilter(width, height));
        return this;
    }

    public FilterChainBuilder Scale(int value, ScaleDimension dimension)
    {
        _filters.Add(new ScaleDimensionFilter(value, dimension));
        return this;
    }

    public FilterChainBuilder Crop(int width, int height, int x, int y)
    {
        _filters.Add(new CropFilter(width, height, x, y));
        return this;
    }

    public FilterChainBuilder HorizontalFlip()
    {
        _filters.Add(new HFlipFilter());
        return this;
    }

    public FilterChainBuilder VerticalFlip()
    {
        _filters.Add(new VFlipFilter());
        return this;
    }

    public FilterChainBuilder Fade(FadeType type, int startFrame, int frames)
    {
        _filters.Add(new FadeFilter(type, startFrame, frames));
        return this;
    }

    public FilterChainBuilder Yadif()
    {
        _filters.Add(new YadifFilter());
        return this;
    }

    public FilterChainBuilder BurnSubtitles(string path)
    {
        _filters.Add(new CustomFilter("subtitles", path));
        return this;
    }

    public FilterChainBuilder Custom(string name, string? args = null)
    {
        _filters.Add(new CustomFilter(name, args));
        return this;
    }

    internal string Render()
    {
        return string.Join(",", _filters.Select(f => f.Render()));
    }
}
