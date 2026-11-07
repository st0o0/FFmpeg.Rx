using FFmpeg.Rx.Internal;

namespace FFmpeg.Rx.Tests;

public sealed class ProbeResultParserTests
{
    private const string SampleJson = """
        {
            "format": {
                "format_name": "matroska,webm",
                "duration": "120.500000",
                "bit_rate": "5000000"
            },
            "streams": [
                {
                    "index": 0,
                    "codec_type": "video",
                    "codec_name": "h264",
                    "width": 1920,
                    "height": 1080,
                    "r_frame_rate": "30000/1001",
                    "bit_rate": "4500000",
                    "pix_fmt": "yuv420p",
                    "duration": "120.500000"
                },
                {
                    "index": 1,
                    "codec_type": "audio",
                    "codec_name": "aac",
                    "sample_rate": "48000",
                    "channels": 2,
                    "bit_rate": "192000",
                    "duration": "120.500000"
                },
                {
                    "index": 2,
                    "codec_type": "subtitle",
                    "codec_name": "srt",
                    "tags": {
                        "language": "eng"
                    }
                }
            ],
            "chapters": [
                {
                    "start_time": "0.000000",
                    "end_time": "30.000000",
                    "tags": { "title": "Intro" }
                },
                {
                    "start_time": "30.000000",
                    "end_time": "120.500000",
                    "tags": { "title": "Main" }
                }
            ]
        }
        """;

    [Fact]
    public void Parses_format_info()
    {
        var info = ProbeResultParser.Parse(SampleJson);
        Assert.Equal("matroska,webm", info.FormatName);
        Assert.Equal(5_000_000, info.Bitrate);
        Assert.Equal(TimeSpan.FromSeconds(120.5), info.Duration);
    }

    [Fact]
    public void Parses_video_stream()
    {
        var info = ProbeResultParser.Parse(SampleJson);
        Assert.Single(info.VideoStreams);
        var v = info.VideoStreams[0];
        Assert.Equal(0, v.Index);
        Assert.Equal("h264", v.Codec);
        Assert.Equal(1920, v.Width);
        Assert.Equal(1080, v.Height);
        Assert.True(Math.Abs(v.Fps - 29.97) < 0.01);
        Assert.Equal(4_500_000, v.Bitrate);
        Assert.Equal("yuv420p", v.PixelFormat);
    }

    [Fact]
    public void Parses_audio_stream()
    {
        var info = ProbeResultParser.Parse(SampleJson);
        Assert.Single(info.AudioStreams);
        var a = info.AudioStreams[0];
        Assert.Equal(1, a.Index);
        Assert.Equal("aac", a.Codec);
        Assert.Equal(48000, a.SampleRate);
        Assert.Equal(2, a.Channels);
        Assert.Equal(192_000, a.Bitrate);
    }

    [Fact]
    public void Parses_subtitle_stream()
    {
        var info = ProbeResultParser.Parse(SampleJson);
        Assert.Single(info.SubtitleStreams);
        var s = info.SubtitleStreams[0];
        Assert.Equal(2, s.Index);
        Assert.Equal("srt", s.Codec);
        Assert.Equal("eng", s.Language);
    }

    [Fact]
    public void Parses_chapters()
    {
        var info = ProbeResultParser.Parse(SampleJson);
        Assert.Equal(2, info.Chapters.Count);
        Assert.Equal("Intro", info.Chapters[0].Title);
        Assert.Equal(TimeSpan.Zero, info.Chapters[0].Start);
        Assert.Equal(TimeSpan.FromSeconds(30), info.Chapters[0].End);
        Assert.Equal("Main", info.Chapters[1].Title);
    }

    [Fact]
    public void Primary_stream_shortcuts()
    {
        var info = ProbeResultParser.Parse(SampleJson);
        Assert.NotNull(info.PrimaryVideo);
        Assert.Equal("h264", info.PrimaryVideo!.Codec);
        Assert.NotNull(info.PrimaryAudio);
        Assert.Equal("aac", info.PrimaryAudio!.Codec);
    }

    [Fact]
    public void Raw_json_preserved()
    {
        var info = ProbeResultParser.Parse(SampleJson);
        Assert.Equal(SampleJson, info.RawJson);
    }

    [Fact]
    public void Minimal_json_with_no_streams()
    {
        var json = """{ "format": { "format_name": "mp4", "duration": "10.0" } }""";
        var info = ProbeResultParser.Parse(json);
        Assert.Equal("mp4", info.FormatName);
        Assert.Empty(info.VideoStreams);
        Assert.Empty(info.AudioStreams);
        Assert.Empty(info.Chapters);
        Assert.Null(info.PrimaryVideo);
    }

    [Fact]
    public void Missing_file_throws_FileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() =>
            Ffprobe.AnalyseAsync("nonexistent_file_that_does_not_exist.mp4", TestContext.Current.CancellationToken).GetAwaiter().GetResult());
    }
}
