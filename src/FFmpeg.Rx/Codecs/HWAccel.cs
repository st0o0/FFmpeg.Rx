namespace FFmpeg.Rx.Codecs;

public readonly record struct HWAccel
{
    public string Name { get; }

    private HWAccel(string name)
    {
        Name = name;
    }

    public static HWAccel Auto => new("auto");
    public static HWAccel Cuda => new("cuda");
    public static HWAccel Qsv => new("qsv");
    public static HWAccel Dxva2 => new("dxva2");
    public static HWAccel D3d11va => new("d3d11va");
    public static HWAccel VideoToolbox => new("videotoolbox");
    public static HWAccel Vaapi => new("vaapi");

    public static HWAccel Custom(string name) => new(name);

    public override string ToString() => Name;
}
