namespace RedBlackTrees.Paper;

/* spell-checker: ignore tikzpicture */

public class LatexSameLevelTreeWriter<K, V>
    where K : IComparable
{
    public void Write(RedBlackTree<K, V> tree, TextWriter writer, double width = 9.0
    )
    {
        var maxLevel = tree.GetMaxLevel();
        var maxLevelNodes = 1 << maxLevel;
        var stepWidth = width / (1.0 + maxLevelNodes);

        writer.WriteLine(@"
\begin{tikzpicture}
\begin{scope}[every node/.style={circle,inner sep=2pt,outer sep=0pt,thick,draw}]");
        PrintLatexSameLevel1(writer, tree, tree.Root, 0, 0, maxLevel, stepWidth);
        writer.Write(@"\end{scope}
\begin{scope}[>={Stealth[black,length=3mm]},
              every node/.style={fill=white,circle},
              red_edge/.style={draw=red,very thick},
              black_edge/.style={draw=black,very thick}]
");
        PrintLatexSameLevel2(writer, tree, tree.Root);
        writer.WriteLine(@"\end{scope}
\end{tikzpicture}
");
    }

    private void PrintLatexSameLevel1(TextWriter writer, RedBlackTree<K, V> tree, Node<K, V> node, double x, double y, int levelDiff, double stepWidth)
    {
        if (tree.IsNil(node)) return;

        writer.WriteLine($"    \\node ({node.Key}) at ({x:F2}, {y:F2}) {{{node.Key}}};");

        if (!tree.IsNil(node.Left))
        {
            var lx = x - levelDiff * stepWidth;
            PrintLatexSameLevel1(writer, tree, node.Left, lx, node.Left.Color == Color.Black ? y - 2 : y, levelDiff - 1, stepWidth);
        }

        if (!tree.IsNil(node.Right))
        {
            var rx = x + levelDiff * stepWidth;
            PrintLatexSameLevel1(writer, tree, node.Right, rx, node.Right.Color == Color.Black ? y - 2 : y, levelDiff - 1, stepWidth);
        }
    }

    private void PrintLatexSameLevel2(TextWriter writer, RedBlackTree<K, V> tree, Node<K, V> node)
    {
        if (tree.IsNil(node)) return;

        if (!tree.IsNil(node.Left))
        {
            var edgeColor = node.Left.Color == Color.Red ? "red_edge" : "black_edge";
            writer.WriteLine($"    \\path [->] ({node.Key}) edge [{edgeColor}] ({{{node.Left.Key}}});");
            PrintLatexSameLevel2(writer, tree, node.Left);
        }

        if (!tree.IsNil(node.Right))
        {
            var edgeColor = node.Right.Color == Color.Red ? "red_edge" : "black_edge";
            writer.WriteLine($"    \\path [->] ({node.Key}) edge [{edgeColor}] ({{{node.Right.Key}}});");
            PrintLatexSameLevel2(writer, tree, node.Right);
        }
    }
}