using System.Text;

namespace FFmpeg.Rx.FilterGraph;

public sealed record FilterGraph(
    IReadOnlyList<GraphInput> Inputs,
    IReadOnlyList<GraphNode> Nodes)
{
    public static FilterGraphBuilder Create() => new();

    public string Serialize()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Nodes.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(';');
            }

            Nodes[i].Serialize(sb);
        }
        return sb.ToString();
    }
}

public sealed record GraphInput(int InputIndex, StreamType Type, string Label);

public abstract record GraphNode
{
    internal abstract void Serialize(StringBuilder sb);
}

public sealed record ChainNode(string InputLabel, string FilterExpression, string OutputLabel) : GraphNode
{
    internal override void Serialize(StringBuilder sb)
    {
        sb.Append('[').Append(InputLabel).Append(']')
            .Append(FilterExpression)
            .Append('[').Append(OutputLabel).Append(']');
    }
}

public sealed record MergeNode(string InputLabel1, string InputLabel2, string FilterExpression, string OutputLabel) : GraphNode
{
    internal override void Serialize(StringBuilder sb)
    {
        sb.Append('[').Append(InputLabel1).Append(']')
            .Append('[').Append(InputLabel2).Append(']')
            .Append(FilterExpression)
            .Append('[').Append(OutputLabel).Append(']');
    }
}

public sealed record SplitNode(string InputLabel, string OutputLabel1, string OutputLabel2, bool IsAudio) : GraphNode
{
    internal override void Serialize(StringBuilder sb)
    {
        sb.Append('[').Append(InputLabel).Append(']')
            .Append(IsAudio ? "asplit" : "split")
            .Append('[').Append(OutputLabel1).Append(']')
            .Append('[').Append(OutputLabel2).Append(']');
    }
}
