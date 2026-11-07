namespace FFmpeg.Rx;

public enum StreamType
{
    Unknown = 0,
    Video = 1,
    Audio = 2,
    Subtitle = 3
}

public enum FailureKind
{
    Unknown = 0,
    Transient = 1,
    Permanent = 2
}

public enum LogLevel
{
    Unknown = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    Fatal = 4
}
