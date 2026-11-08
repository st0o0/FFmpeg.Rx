using FFmpeg.Rx.Builder;
using FFmpeg.Rx.Codecs;
using FFmpeg.Rx.Models;

namespace FFmpeg.Rx.Tests.Builder;

public sealed class StreamBuilderTests
{
    [Fact]
    public void Copy_produces_StreamCopy()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Video);
        builder.Copy();
        var spec = builder.Build();
        Assert.IsType<StreamCopy>(spec.Codec);
        Assert.Equal(0, spec.InputIndex);
        Assert.Equal(0, spec.StreamIndex);
        Assert.Equal(StreamType.Video, spec.Type);
    }

    [Fact]
    public void Encode_video_produces_VideoEncode()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Video);
        builder.Encode(VideoCodec.H264, e => e.Quality(Quality.Crf(23)));
        var spec = builder.Build();
        var ve = Assert.IsType<VideoEncode>(spec.Codec);
        Assert.Equal(VideoCodec.H264, ve.Codec);
        Assert.IsType<ConstantQuality>(ve.Options.Quality);
    }

    [Fact]
    public void Encode_audio_produces_AudioEncode()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Audio);
        builder.Encode(AudioCodec.Aac, e => e.Bitrate(Bitrate.Kbps(192)));
        var spec = builder.Build();
        var ae = Assert.IsType<AudioEncode>(spec.Codec);
        Assert.Equal(AudioCodec.Aac, ae.Codec);
        Assert.Equal(Bitrate.Kbps(192), ae.Options.Bitrate);
    }

    [Fact]
    public void Encode_subtitle_produces_SubtitleEncode()
    {
        var builder = new StreamBuilder(1, 0, StreamType.Subtitle);
        builder.Encode(SubtitleCodec.Srt);
        var spec = builder.Build();
        var se = Assert.IsType<SubtitleEncode>(spec.Codec);
        Assert.Equal(SubtitleCodec.Srt, se.Codec);
    }

    [Fact]
    public void Metadata_adds_entries()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Audio);
        builder.Copy();
        builder.Metadata("language", "eng");
        var spec = builder.Build();
        Assert.NotNull(spec.Metadata);
        var entry = Assert.Single(spec.Metadata);
        Assert.Equal("language", entry.Key);
        Assert.Equal("eng", entry.Value);
    }

    [Fact]
    public void Disposition_sets_value()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Audio);
        builder.Copy();
        builder.Disposition("default");
        var spec = builder.Build();
        Assert.Equal("default", spec.Disposition);
    }

    [Fact]
    public void Null_stream_index()
    {
        var builder = new StreamBuilder(0, null, StreamType.Video);
        builder.Copy();
        var spec = builder.Build();
        Assert.Null(spec.StreamIndex);
    }

    [Fact]
    public void Default_codec_is_StreamCopy()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Video);
        var spec = builder.Build();
        Assert.IsType<StreamCopy>(spec.Codec);
    }

    [Fact]
    public void Encode_video_without_configure()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Video);
        builder.Encode(VideoCodec.H264);
        var spec = builder.Build();
        var ve = Assert.IsType<VideoEncode>(spec.Codec);
        Assert.Null(ve.Options.Quality);
    }

    [Fact]
    public void Encode_audio_without_configure()
    {
        var builder = new StreamBuilder(0, 0, StreamType.Audio);
        builder.Encode(AudioCodec.Aac);
        var spec = builder.Build();
        var ae = Assert.IsType<AudioEncode>(spec.Codec);
        Assert.Null(ae.Options.Bitrate);
    }
}
