namespace RedBlackTrees.Paper;

public class ConsoleTreeWriter<K, V>
    where K : IComparable
{
    public void Write(RedBlackTree<K, V> tree, TextWriter writer)
        => PrintConsole(writer, tree, tree.Root, "", true);

    private void PrintConsole(TextWriter writer, RedBlackTree<K, V> tree, Node<K, V> node, string indent, bool last)
    {
        if (tree.IsNil(node)) return;

        writer.WriteLine($"{indent}+- {node.Color.ToString()[0]}{node.Key}");
        indent += last ? "   " : "|  ";

        if (!tree.IsNil(node.Left)) PrintConsole(writer, tree, node.Left, indent, false);
        if (!tree.IsNil(node.Right)) PrintConsole(writer, tree, node.Right, indent, true);
    }
}