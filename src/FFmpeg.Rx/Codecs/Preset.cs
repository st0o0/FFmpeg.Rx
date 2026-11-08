namespace FFmpeg.Rx.Codecs;

public readonly record struct Preset
{
    public string Name { get; }

    private Preset(string name)
    {
        Name = name;
    }

    public static Preset UltraFast => new("ultrafast");
    public static Preset SuperFast => new("superfast");
    public static Preset VeryFast => new("veryfast");
    public static Preset Faster => new("faster");
    public static Preset Fast => new("fast");
    public static Preset Medium => new("medium");
    public static Preset Slow => new("slow");
    public static Preset Slower => new("slower");
    public static Preset VerySlow => new("veryslow");

    public static Preset Custom(string name) => new(name);

    public override string ToString() => Name;
}
