namespace RedBlackTrees.Paper;

public class ConsoleTreeWriter<K, V>
    where K : IComparable
{
    public void Write(RedBlackTree<K, V> tree, TextWriter writer)
        => PrintConsole(writer, tree, tree.Root, null, "", true);

    private void PrintConsole(TextWriter writer, RedBlackTree<K, V> tree, Node<K, V> node, Node<K, V>? parent, string indent, bool last)
    {
        if (tree.IsNil(node)) return;

        var dir = parent == null ? "" : node.IsLeftChildOf(parent) ? "0" : "1";
        writer.WriteLine($"{indent}{dir}- {node.Color.ToString()[0]}{node.Key}");
        indent += last ? "   " : "|  ";

        if (!tree.IsNil(node.Left)) PrintConsole(writer, tree, node.Left, node, indent, false);
        if (!tree.IsNil(node.Right)) PrintConsole(writer, tree, node.Right, node, indent, true);
    }
}