namespace FFmpeg.Rx.Events;

public sealed record PercentageProgressEvent(double Percent, ProgressEvent Source) : FfmpegEvent;
