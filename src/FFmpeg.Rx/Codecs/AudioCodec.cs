namespace FFmpeg.Rx.Codecs;

public readonly record struct AudioCodec
{
    public string Name { get; }

    private AudioCodec(string name) => Name = name;

    public static AudioCodec Aac => new("aac");
    public static AudioCodec Opus => new("libopus");
    public static AudioCodec Mp3 => new("libmp3lame");
    public static AudioCodec Flac => new("flac");
    public static AudioCodec Pcm16Le => new("pcm_s16le");
    public static AudioCodec Pcm24Le => new("pcm_s24le");
    public static AudioCodec Vorbis => new("libvorbis");
    public static AudioCodec Copy => new("copy");

    public static AudioCodec Custom(string name) => new(name);

    public override string ToString() => Name;
}
