using FFmpeg.Rx.Filters;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class VideoFilterBuilderTests
{
    [Fact]
    public void Scale_adds_filter()
    {
        var builder = new VideoFilterBuilder();
        builder.Scale(1920, 1080);
        Assert.Single(builder.Filters);
        Assert.IsType<ScaleFilter>(builder.Filters[0]);
    }

    [Fact]
    public void Scale_dimension_width()
    {
        var builder = new VideoFilterBuilder();
        builder.Scale(1920, ScaleDimension.Width);
        var filter = Assert.IsType<ScaleDimensionFilter>(Assert.Single(builder.Filters));
        Assert.Equal("scale=1920:-2", filter.Render());
    }

    [Fact]
    public void Scale_dimension_height()
    {
        var builder = new VideoFilterBuilder();
        builder.Scale(720, ScaleDimension.Height);
        var filter = Assert.IsType<ScaleDimensionFilter>(Assert.Single(builder.Filters));
        Assert.Equal("scale=-2:720", filter.Render());
    }

    [Fact]
    public void Crop_adds_filter()
    {
        var builder = new VideoFilterBuilder();
        builder.Crop(640, 480, 100, 50);
        var filter = Assert.IsType<CropFilter>(Assert.Single(builder.Filters));
        Assert.Equal("crop=640:480:100:50", filter.Render());
    }

    [Fact]
    public void Crop_expression()
    {
        var builder = new VideoFilterBuilder();
        builder.Crop("iw/2:ih/2");
        var filter = Assert.IsType<CropExprFilter>(Assert.Single(builder.Filters));
        Assert.Equal("crop=iw/2:ih/2", filter.Render());
    }

    [Fact]
    public void HorizontalFlip()
    {
        var builder = new VideoFilterBuilder();
        builder.HorizontalFlip();
        var filter = Assert.IsType<HFlipFilter>(Assert.Single(builder.Filters));
        Assert.Equal("hflip", filter.Render());
    }

    [Fact]
    public void VerticalFlip()
    {
        var builder = new VideoFilterBuilder();
        builder.VerticalFlip();
        var filter = Assert.IsType<VFlipFilter>(Assert.Single(builder.Filters));
        Assert.Equal("vflip", filter.Render());
    }

    [Fact]
    public void Transpose()
    {
        var builder = new VideoFilterBuilder();
        builder.Transpose(TransposeMode.Clock);
        var filter = Assert.IsType<TransposeFilter>(Assert.Single(builder.Filters));
        Assert.Equal("transpose=1", filter.Render());
    }

    [Fact]
    public void Fade_by_frame()
    {
        var builder = new VideoFilterBuilder();
        builder.Fade(FadeType.In, 0, 30);
        var filter = Assert.IsType<FadeFilter>(Assert.Single(builder.Filters));
        Assert.Equal("fade=t=in:s=0:n=30", filter.Render());
    }

    [Fact]
    public void Fade_by_time()
    {
        var builder = new VideoFilterBuilder();
        builder.Fade(FadeType.Out, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2));
        var filter = Assert.IsType<FadeFilter>(Assert.Single(builder.Filters));
        Assert.Equal("fade=t=out:st=5.000:d=2.000", filter.Render());
    }

    [Fact]
    public void Pad()
    {
        var builder = new VideoFilterBuilder();
        builder.Pad(1920, 1080, 0, 0, "black");
        var filter = Assert.IsType<PadFilter>(Assert.Single(builder.Filters));
        Assert.Equal("pad=1920:1080:0:0:black", filter.Render());
    }

    [Fact]
    public void Yadif()
    {
        var builder = new VideoFilterBuilder();
        builder.Yadif();
        var filter = Assert.IsType<YadifFilter>(Assert.Single(builder.Filters));
        Assert.Equal("yadif", filter.Render());
    }

    [Fact]
    public void Custom()
    {
        var builder = new VideoFilterBuilder();
        builder.Custom("drawtext", "text='hello'");
        var filter = Assert.IsType<CustomFilter>(Assert.Single(builder.Filters));
        Assert.Equal("drawtext=text='hello'", filter.Render());
    }

    [Fact]
    public void Custom_without_args()
    {
        var builder = new VideoFilterBuilder();
        builder.Custom("null");
        var filter = Assert.IsType<CustomFilter>(Assert.Single(builder.Filters));
        Assert.Equal("null", filter.Render());
    }

    [Fact]
    public void Multiple_filters_chain()
    {
        var builder = new VideoFilterBuilder();
        builder.Scale(1920, 1080).HorizontalFlip().Yadif();
        Assert.Equal(3, builder.Filters.Count);
    }
}
