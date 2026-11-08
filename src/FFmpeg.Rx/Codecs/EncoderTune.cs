namespace FFmpeg.Rx.Codecs;

public readonly record struct Tune
{
    public string Name { get; }

    private Tune(string name) => Name = name;

    public static Tune Film => new("film");
    public static Tune Animation => new("animation");
    public static Tune Grain => new("grain");
    public static Tune StillImage => new("stillimage");
    public static Tune FastDecode => new("fastdecode");
    public static Tune ZeroLatency => new("zerolatency");

    public static Tune Custom(string name)
    {
        return new Tune(name);
    }

    public override string ToString()
    {
        return Name;
    }
}
