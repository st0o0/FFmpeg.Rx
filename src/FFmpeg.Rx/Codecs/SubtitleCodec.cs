namespace FFmpeg.Rx.Codecs;

public readonly record struct SubtitleCodec
{
    public string Name { get; }

    private SubtitleCodec(string name) => Name = name;

    public static SubtitleCodec MovText => new("mov_text");
    public static SubtitleCodec Srt => new("srt");
    public static SubtitleCodec Ass => new("ass");
    public static SubtitleCodec WebVtt => new("webvtt");
    public static SubtitleCodec Copy => new("copy");

    public static SubtitleCodec Custom(string name)
    {
        return new SubtitleCodec(name);
    }

    public override string ToString()
    {
        return Name;
    }
}
