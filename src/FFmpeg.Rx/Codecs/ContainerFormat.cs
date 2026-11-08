namespace FFmpeg.Rx.Codecs;

public readonly record struct ContainerFormat
{
    public string Name { get; }

    private ContainerFormat(string name)
    {
        Name = name;
    }

    public static ContainerFormat Mp4 => new("mp4");
    public static ContainerFormat Mkv => new("matroska");
    public static ContainerFormat WebM => new("webm");
    public static ContainerFormat Ts => new("mpegts");
    public static ContainerFormat Flv => new("flv");
    public static ContainerFormat Avi => new("avi");
    public static ContainerFormat Mov => new("mov");
    public static ContainerFormat Ogg => new("ogg");
    public static ContainerFormat Wav => new("wav");
    public static ContainerFormat Mp3 => new("mp3");
    public static ContainerFormat Flac => new("flac");
    public static ContainerFormat Null => new("null");

    public static ContainerFormat Custom(string name) => new(name);

    public override string ToString() => Name;
}
