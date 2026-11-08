namespace FFmpeg.Rx.FilterGraph;

public sealed class MergeFilterBuilder
{
    private string? _filter;

    public MergeFilterBuilder Overlay(int x, int y)
    {
        _filter = $"overlay={x}:{y}";
        return this;
    }

    public MergeFilterBuilder Overlay(string xExpr, string yExpr)
    {
        _filter = $"overlay={xExpr}:{yExpr}";
        return this;
    }

    public MergeFilterBuilder Concat(int segments = 2, bool hasVideo = true, bool hasAudio = true)
    {
        var v = hasVideo ? 1 : 0;
        var a = hasAudio ? 1 : 0;
        _filter = $"concat=n={segments}:v={v}:a={a}";
        return this;
    }

    public MergeFilterBuilder AudioMix(int inputs = 2)
    {
        _filter = $"amix=inputs={inputs}";
        return this;
    }

    public MergeFilterBuilder Custom(string expression)
    {
        _filter = expression;
        return this;
    }

    internal string Render()
    {
        return _filter ?? throw new InvalidOperationException("No merge filter specified");
    }
}
