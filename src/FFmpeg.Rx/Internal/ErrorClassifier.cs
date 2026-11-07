namespace FFmpeg.Rx.Internal;

internal static class ErrorClassifier
{
    internal static FailureKind Classify(string? error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            return FailureKind.Permanent;
        }

        if (error.Contains("503", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("502", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("timed out", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("Connection reset", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("Connection refused", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("Network is unreachable", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("Temporary failure", StringComparison.OrdinalIgnoreCase))
        {
            return FailureKind.Transient;
        }

        return FailureKind.Permanent;
    }

    internal static string ExtractError(string? stderr)
    {
        if (string.IsNullOrWhiteSpace(stderr))
        {
            return "";
        }

        var lines = stderr.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i].Trim();
            if (line.Contains("HTTP error", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Error opening input", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Server returned", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Invalid data found", StringComparison.OrdinalIgnoreCase))
            {
                return line;
            }
        }

        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i].Trim();
            if (line.Length > 0)
            {
                return line;
            }
        }

        return "";
    }
}
