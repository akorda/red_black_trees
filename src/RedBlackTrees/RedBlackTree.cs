using System.Diagnostics;

namespace RedBlackTrees.Paper;

public enum Color : byte
{
    Red,
    Black,
}

[DebuggerDisplay("{GetDebuggerDisplay()}")]
public class Node<K, V>
{
    public K Key { get; set; }
    public V Value { get; set; }
    public Color Color { get; set; }
    public Node<K, V> Left { get; set; }
    public Node<K, V> Right { get; set; }

    public Node(K key, V value, Node<K, V> nil)
    {
        Key = key;
        Value = value;
        Color = Color.Red;
        Left = nil;
        Right = nil;
    }

    public static Node<K, V> CreateNil()
        => new(default!, default!, null!) { Color = Color.Black };

    public bool IsRed => Color == Color.Red;
    public bool IsBlack => Color == Color.Black;

    public bool IsLeftChildOf(Node<K, V>? parent) => parent?.Left == this;
    public bool IsRightChildOf(Node<K, V>? parent) => parent?.Right == this;

    private string GetDebuggerDisplay()
        => $"{Color.ToString()[0]}: {Key}";
}


public class RedBlackTree<K, V>
    where K : IComparable
{
    [DebuggerDisplay("{GetDebuggerDisplay()}")]
    private class Path
    {
        public List<Node<K, V>> Nodes { get; private set; } = [];
        public int Index { get; private set; }

        public void Push(Node<K, V> node)
        {
            Nodes.Add(node);
            Index++;
        }

        public Node<K, V> Pop()
        {
            var node = Nodes[--Index];
            Nodes.RemoveAt(Index);
            return node;
        }

        public Node<K, V> Peek()
            => Nodes[Index - 1];

        public Node<K, V> PeekParent()
            => Nodes[Index - 2];

        public Node<K, V> PeekGrandParent()
            => Nodes[Index - 3];

        public Node<K, V> PeekGreatGrandParent()
            => Nodes[Index - 4];

        private string GetDebuggerDisplay()
        {
            if (Index == 0) return "-";
            var node = Peek();
            return $"{node.Color.ToString()[0]}: {node.Key}";
        }
    }

    public Node<K, V> Root { get; private set; }

    private readonly Node<K, V> Nil = Node<K, V>.CreateNil();
    public int Size { get; private set; }

    public RedBlackTree()
    {
        Root = Nil;
    }

    public void Insert(K key, V value)
    {
        var x = Root;
        var y = Nil;
        int compare = 0;

        var path = new Path();
        path.Push(Nil);

        while (x != Nil)
        {
            path.Push(x);

            y = x;
            compare = key.CompareTo(x.Key);

            if (compare == 0)
            {
                x.Value = value;
                return;
            }

            if (compare < 0) x = x.Left;
            else x = x.Right;
        }

        var z = new Node<K, V>(key, value, Nil);

        if (y == Nil) Root = z;
        else if (compare < 0) y.Left = z;
        else y.Right = z;

        path.Push(z);
        InsertFixup(path);

        Size++;
    }

    private void InsertFixup(Path path)
    {
        Node<K, V> p;

        while ((p = path.PeekParent()).IsRed)
        {
            var z = path.Peek();
            var gp = path.PeekGrandParent();

            if (p.IsLeftChildOf(gp))
            {
                var uncle = gp.Right;

                if (uncle.IsRed)
                {
                    // Case 1
                    p.Color = Color.Black;
                    uncle.Color = Color.Black;
                    gp.Color = Color.Red;
                    path.Pop();
                    path.Pop();
                }
                else
                {
                    if (z.IsRightChildOf(p))
                    {
                        // Case 2.1
                        if (Root == gp)
                        {
                            Root = z;
                        }
                        else
                        {
                            var gpp = path.PeekGreatGrandParent();
                            if (gp.IsLeftChildOf(gpp)) gpp.Left = z;
                            else gpp.Right = z;
                        }

                        p.Right = z.Left;
                        gp.Left = z.Right;
                        z.Left = p;
                        p.Color = Color.Red;
                        z.Right = gp;
                        gp.Color = Color.Red;
                        z.Color = Color.Black;
                        break;
                    }
                    else
                    {
                        // Case 2.2
                        if (Root == gp)
                        {
                            Root = p;
                        }
                        else
                        {
                            var gpp = path.PeekGreatGrandParent();
                            if (gp.IsLeftChildOf(gpp)) gpp.Left = p;
                            else gpp.Right = p;
                        }

                        gp.Left = p.Right;
                        p.Right = gp;
                        gp.Color = Color.Red;
                        p.Color = Color.Black;
                        break;
                    }
                }
            }
            else
            {
                var uncle = gp.Left;

                if (uncle.IsRed)
                {
                    // Case 1
                    p.Color = Color.Black;
                    uncle.Color = Color.Black;
                    gp.Color = Color.Red;
                    path.Pop();
                    path.Pop();
                }
                else
                {
                    if (z.IsLeftChildOf(p))
                    {
                        // Case 2.1
                        if (Root == gp)
                        {
                            Root = z;
                        }
                        else
                        {
                            var gpp = path.PeekGreatGrandParent();
                            if (gp.IsLeftChildOf(gpp)) gpp.Left = z;
                            else gpp.Right = z;
                        }

                        p.Left = z.Right;
                        gp.Right = z.Left;
                        z.Right = p;
                        p.Color = Color.Red;
                        z.Left = gp;
                        gp.Color = Color.Red;
                        z.Color = Color.Black;
                        break;
                    }
                    else
                    {
                        // Case 2.2
                        if (Root == gp)
                        {
                            Root = p;
                        }
                        else
                        {
                            var gpp = path.PeekGreatGrandParent();
                            if (gp.IsLeftChildOf(gpp)) gpp.Left = p;
                            else gpp.Right = p;
                        }

                        gp.Right = p.Left;
                        p.Left = gp;
                        gp.Color = Color.Red;
                        p.Color = Color.Black;
                        break;
                    }
                }
            }
        }

        Root!.Color = Color.Black;
    }

    public V? Search(K key)
    {
        // Set the key of the Nil node to the value of 
        // the key that we search. The value of the Nil
        // node is the default V value
        Nil.Key = key;

        return Search(Root, key);
    }

    private static V? Search(Node<K, V> node, K key)
    {
        var compare = key.CompareTo(node.Key);

        if (compare == 0) return node.Value;
        if (compare < 0) return Search(node.Left, key);
        return Search(node.Right, key);
    }

    public V? IterativeSearch(K key)
    {
        // Set the key of the Nil node to the value of 
        // the key that we search. The value of the Nil
        // node is the default V value
        Nil.Key = key;

        int compare;
        var node = Root;
        while ((compare = key.CompareTo(node.Key)) != 0)
        {
            if (compare < 0) node = node.Left;
            else node = node.Right;
        }

        return node.Value;
    }

    public int GetMaxLevel()
    {
        var level = 0;
        var n = Root;
        while (n != Nil)
        {
            level++;
            n = n.Left;
        }

        return level;
    }

    public bool IsNil(Node<K, V> node)
        => node == Nil;
}
