using FFmpeg.Rx.Internal;

namespace FFmpeg.Rx.FilterGraph;

public sealed class FilterGraphBuilder
{
    private readonly List<GraphInput> _inputs = [];
    private readonly List<GraphNode> _nodes = [];

    public FilterGraphBuilder Input(int inputIndex, StreamType type, string label)
    {
        _inputs.Add(new GraphInput(inputIndex, type, label));
        return this;
    }

    public FilterGraphBuilder Chain(string inputLabel, Action<FilterChainBuilder> configure, string outputLabel)
    {
        var builder = new FilterChainBuilder();
        configure(builder);
        _nodes.Add(new ChainNode(ResolveLabel(inputLabel), builder.Render(), outputLabel));
        return this;
    }

    public FilterGraphBuilder Merge(string label1, string label2, Action<MergeFilterBuilder> configure, string outputLabel)
    {
        var builder = new MergeFilterBuilder();
        configure(builder);
        _nodes.Add(new MergeNode(label1, label2, builder.Render(), outputLabel));
        return this;
    }

    public FilterGraphBuilder Split(string inputLabel, string outputLabel1, string outputLabel2)
    {
        _nodes.Add(new SplitNode(inputLabel, outputLabel1, outputLabel2, false));
        return this;
    }

    public FilterGraphBuilder ASplit(string inputLabel, string outputLabel1, string outputLabel2)
    {
        _nodes.Add(new SplitNode(inputLabel, outputLabel1, outputLabel2, true));
        return this;
    }

    public FilterGraph Build()
    {
        return new FilterGraph(_inputs.AsReadOnly(), _nodes.AsReadOnly());
    }

    private string ResolveLabel(string label)
    {
        var input = _inputs.Find(i => i.Label == label);
        if (input is not null)
        {
            return $"{input.InputIndex}:{ArgumentBuilder.StreamTypeChar(input.Type)}";
        }
        return label;
    }
}
