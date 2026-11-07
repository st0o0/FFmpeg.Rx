namespace FFmpeg.Rx.Filters;

public sealed class VideoFilterBuilder
{
    private readonly List<IVideoFilter> _filters = [];

    internal IReadOnlyList<IVideoFilter> Filters => _filters;

    public VideoFilterBuilder Scale(int width, int height)
    {
        _filters.Add(new ScaleFilter(width, height));
        return this;
    }

    public VideoFilterBuilder Scale(int value, ScaleDimension dimension)
    {
        _filters.Add(new ScaleDimensionFilter(value, dimension));
        return this;
    }

    public VideoFilterBuilder Crop(int width, int height, int x, int y)
    {
        _filters.Add(new CropFilter(width, height, x, y));
        return this;
    }

    public VideoFilterBuilder Crop(string expression)
    {
        _filters.Add(new CropExprFilter(expression));
        return this;
    }

    public VideoFilterBuilder HorizontalFlip()
    {
        _filters.Add(new HFlipFilter());
        return this;
    }

    public VideoFilterBuilder VerticalFlip()
    {
        _filters.Add(new VFlipFilter());
        return this;
    }

    public VideoFilterBuilder Transpose(TransposeMode mode)
    {
        _filters.Add(new TransposeFilter(mode));
        return this;
    }

    public VideoFilterBuilder Fade(FadeType type, int startFrame, int frames)
    {
        _filters.Add(new FadeFilter(type, startFrame, frames));
        return this;
    }

    public VideoFilterBuilder Fade(FadeType type, TimeSpan start, TimeSpan duration)
    {
        _filters.Add(new FadeFilter(type, start, duration));
        return this;
    }

    public VideoFilterBuilder Pad(int width, int height, int x, int y, string color)
    {
        _filters.Add(new PadFilter(width, height, x, y, color));
        return this;
    }

    public VideoFilterBuilder Yadif()
    {
        _filters.Add(new YadifFilter());
        return this;
    }

    public VideoFilterBuilder Custom(string name, string? args = null)
    {
        _filters.Add(new CustomFilter(name, args));
        return this;
    }
}
