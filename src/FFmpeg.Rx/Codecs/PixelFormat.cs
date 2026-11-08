namespace FFmpeg.Rx.Codecs;

public readonly record struct PixelFormat
{
    public string Name { get; }

    private PixelFormat(string name)
    {
        Name = name;
    }

    public static PixelFormat Yuv420p => new("yuv420p");
    public static PixelFormat Yuv422p => new("yuv422p");
    public static PixelFormat Yuv444p => new("yuv444p");
    public static PixelFormat Rgb24 => new("rgb24");
    public static PixelFormat Rgba => new("rgba");
    public static PixelFormat Nv12 => new("nv12");
    public static PixelFormat Nv21 => new("nv21");
    public static PixelFormat P010Le => new("p010le");

    public static PixelFormat Custom(string name) => new(name);

    public override string ToString() => Name;
}
