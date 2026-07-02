namespace RedBlackTrees.Paper;

/* spell-checker: ignore tikzpicture */

public class LatexTreeWriter<K, V>
    where K : IComparable
{
    public void Write(RedBlackTree<K, V> tree, TextWriter writer, double width = 120.0)
    {
        var maxLevel = tree.GetMaxLevel();

        writer.WriteLine(@"
\begin{tikzpicture}[");
        for (var level = 1; level <= maxLevel; level++)
        {
            var levelNodes = 1 << level;
            var distance = width / (double)levelNodes;
            writer.WriteLine($"  level {level}/.style={{sibling distance={distance:F2}mm}},");
        }

        writer.WriteLine(@"  nil_node/.style={rectangle,rounded corners=2mm,draw,black,fill=black,text=white,minimum size=8mm,inner sep=0pt},
  red_node/.style={circle,draw,red,fill=red,text=white,minimum size=8mm,inner sep=0pt},
  black_node/.style={circle,draw,black,fill=black,text=white,minimum size=8mm,inner sep=0pt},
]"
        );
        writer.WriteLine($"\\node [black_node] {{{tree.Root.Key}}}");
        if (!tree.IsNil(tree.Root.Left)) PrintLatex(writer, tree, tree.Root.Left, "");
        if (!tree.IsNil(tree.Root.Right)) PrintLatex(writer, tree, tree.Root.Right, "");
        writer.WriteLine(@";
\end{tikzpicture}");
    }

    private void PrintLatex(TextWriter writer, RedBlackTree<K, V> tree, Node<K, V> node, string indent)
    {
        if (tree.IsNil(node)) return;

        indent += "    ";
        var type = node.Color == Color.Red ? "red_node" : "black_node";

        writer.Write($"{indent}child {{node [{type}] {{{node.Key}}}");

        if (!tree.IsNil(node.Left) || !tree.IsNil(node.Right))
        {
            writer.WriteLine();

            if (!tree.IsNil(node.Left)) PrintLatex(writer, tree, node.Left, indent);
            else writer.WriteLine($"{indent}    child [missing]");

            if (!tree.IsNil(node.Right)) PrintLatex(writer, tree, node.Right, indent);
            else writer.WriteLine($"{indent}    child [missing]");

            writer.WriteLine($"{indent}}}");
        }
        else
        {
            writer.WriteLine($"}}");
        }
    }
}
