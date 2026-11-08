namespace FFmpeg.Rx.Codecs;

public readonly record struct VideoProfile
{
    public string Name { get; }

    private VideoProfile(string name) => Name = name;

    public static VideoProfile Baseline => new("baseline");
    public static VideoProfile Main => new("main");
    public static VideoProfile High => new("high");
    public static VideoProfile High10 => new("high10");
    public static VideoProfile High422 => new("high422");
    public static VideoProfile High444 => new("high444");

    public static VideoProfile Custom(string name)
    {
        return new VideoProfile(name);
    }

    public override string ToString()
    {
        return Name;
    }
}
