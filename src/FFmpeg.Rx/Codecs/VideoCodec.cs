namespace FFmpeg.Rx.Codecs;

public readonly record struct VideoCodec
{
    public string Name { get; }

    private VideoCodec(string name) => Name = name;

    public static VideoCodec H264 => new("libx264");
    public static VideoCodec Hevc => new("libx265");
    public static VideoCodec Vp9 => new("libvpx-vp9");
    public static VideoCodec Av1 => new("libaom-av1");
    public static VideoCodec SvtAv1 => new("libsvtav1");
    public static VideoCodec H264Nvenc => new("h264_nvenc");
    public static VideoCodec HevcNvenc => new("hevc_nvenc");
    public static VideoCodec H264Qsv => new("h264_qsv");
    public static VideoCodec HevcQsv => new("hevc_qsv");
    public static VideoCodec H264Amf => new("h264_amf");
    public static VideoCodec HevcAmf => new("hevc_amf");
    public static VideoCodec H264VideoToolbox => new("h264_videotoolbox");
    public static VideoCodec HevcVideoToolbox => new("hevc_videotoolbox");
    public static VideoCodec H264Cuvid => new("h264_cuvid");
    public static VideoCodec Copy => new("copy");

    public static VideoCodec Custom(string name) => new(name);

    public override string ToString() => Name;
}
