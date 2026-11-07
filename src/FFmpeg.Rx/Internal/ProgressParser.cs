using System.Globalization;
using FFmpeg.Rx.Events;

namespace FFmpeg.Rx.Internal;

internal static class ProgressParser
{
    internal static ProgressEvent? ParseLine(string line, Dictionary<string, string> block)
    {
        var eqIndex = line.IndexOf('=');
        if (eqIndex <= 0)
        {
            return null;
        }

        var key = line[..eqIndex].Trim();
        var value = line[(eqIndex + 1)..].Trim();
        block[key] = value;

        if (key != "progress")
        {
            return null;
        }

        var result = ToEvent(block);
        block.Clear();
        return result;
    }

    private static ProgressEvent ToEvent(Dictionary<string, string> block)
    {
        var totalSize = GetLong(block, "total_size");
        var outTimeUs = GetLong(block, "out_time_us");
        var speed = ParseSpeed(block.GetValueOrDefault("speed"));
        return new ProgressEvent(totalSize, outTimeUs, speed);
    }

    private static long GetLong(Dictionary<string, string> block, string key)
    {
        return block.TryGetValue(key, out var value) &&
               long.TryParse(value, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    private static double ParseSpeed(string? value)
    {
        if (value is null or "N/A")
        {
            return 0.0;
        }

        var trimmed = value.TrimEnd('x');
        return double.TryParse(trimmed, CultureInfo.InvariantCulture, out var result) ? result : 0.0;
    }
}
