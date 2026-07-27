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
    public V? Value { get; set; }
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

    public void Preorder(Action<Node<K, V>> visit)
    {
        ArgumentNullException.ThrowIfNull(visit);

        PreorderInt(Root, visit);
    }

    private void PreorderInt(Node<K, V> node, Action<Node<K, V>> visit)
    {
        if (node == Nil) return;

        visit(node);
        PreorderInt(node.Left, visit);
        PreorderInt(node.Right, visit);
    }

    public void Inorder(Action<Node<K, V>> visit)
    {
        ArgumentNullException.ThrowIfNull(visit);

        InorderInt(Root, visit);
    }

    private void InorderInt(Node<K, V> node, Action<Node<K, V>> visit)
    {
        if (node == Nil) return;

        InorderInt(node.Left, visit);
        visit(node);
        InorderInt(node.Right, visit);
    }

    public void Postorder(Action<Node<K, V>> visit)
    {
        ArgumentNullException.ThrowIfNull(visit);

        PostorderInt(Root, visit);
    }

    private void PostorderInt(Node<K, V> node, Action<Node<K, V>> visit)
    {
        if (node == Nil) return;

        PostorderInt(node.Left, visit);
        PostorderInt(node.Right, visit);
        visit(node);
    }

    public (K, V)? GetMinimum()
        => GetMinimumInt(Root);

    private (K, V)? GetMinimumInt(Node<K, V> node)
    {
        if (node == Nil) return null;

        while (node.Left != Nil)
            node = node.Left;

        return (node.Key, node.Value!);
    }

    public (K, V)? GetMaximum()
        => GetMaximumInt(Root);

    private (K, V)? GetMaximumInt(Node<K, V> node)
    {
        if (node == Nil) return null;

        while (node.Right != Nil)
            node = node.Right;

        return (node.Key, node.Value!);
    }

    /// <summary>
    /// path.Peek() contains the minimum node
    /// </summary>
    /// <param name="node"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private Path GetPathToMinimum(Node<K, V> node, Node<K, V> parent)
    {
        if (node == Nil) throw new InvalidOperationException();

        var path = new Path();
        path.Push(parent);

        while (node.Left != Nil)
        {
            path.Push(node);
            node = node.Left;
        }

        path.Push(node);

        return path;
    }

    private void Transplant(
        Node<K, V> oldNode,
        Node<K, V> oldNodeParent,
        Node<K, V> newNode,
        Node<K, V> newNodeParent
    )
    {
        // Detach new node from it's parent
        if (newNode.IsLeftChildOf(newNodeParent)) newNodeParent.Left = Nil;
        else if (newNode.IsRightChildOf(newNodeParent)) newNodeParent.Right = Nil;

        // Attach new node to old node's parent or Root
        if (IsNil(oldNodeParent)) Root = newNode;
        else if (oldNode.IsLeftChildOf(oldNodeParent)) oldNodeParent.Left = newNode;
        else oldNodeParent.Right = newNode;
    }

    public bool Delete(K key)
    {
        var x = Root;

        var path = new Path();
        path.Push(Nil);

        while (x != Nil)
        {
            path.Push(x);

            var compare = key.CompareTo(x.Key);

            if (compare == 0)
            {
                break;
            }

            if (compare < 0) x = x.Left;
            else x = x.Right;
        }

        return Delete(path);
    }

    private bool Delete(Path path)
    {
        var node = path.Peek();

        if (node == Nil) return false;

        var parent = path.PeekParent();

        Node<K, V> child;
        var minNode = node;
        var nodeColor = minNode.Color;

        if (node.Left == Nil)
        {
            child = node.Right;
            Transplant(node, parent, node.Right, node);
        }
        else if (node.Right == Nil)
        {
            child = node.Left;
            Transplant(node, parent, node.Left, node);
        }
        else
        {
            var minPath = GetPathToMinimum(node.Right, node);
            minNode = minPath.Peek();
            nodeColor = minNode.Color;
            var minNodeParent = minPath.PeekParent();

            // child = minNode.Right;
            child = minNode;

            if (minNode != node.Right)
            {
                Transplant(minNode, minNodeParent, minNode.Right, minNode);
                minNode.Right = node.Right;
                // nextNode.Right.Parent = nextNode;
            }
            else
            {
                // child.Parent = nextNode;
            }

            Transplant(node, parent, minNode, minNodeParent);
            minNode.Left = node.Left;
            // nextNode.Left.Parent = nextNode;
            minNode.Color = node.Color;

            // path.Pop();
            // path.Push(minPath.Pop());
            // for (var i = 1; i < minPath.Nodes.Count; i++)
            // {
            //     path.Push(minPath.Nodes[i]);
            // }
            // path.Push(child);
        }

        if (nodeColor == Color.Black)
        {
            path.Pop();
            path.Push(child);
            DeleteFixup(path);
        }

        Size--;

        return true;
    }

    private void DeleteFixup(Path path)
    {
        var node = path.Peek();

        while (node != Root && node.IsBlack)
        {
            var parent = path.PeekParent();

            if (node.IsLeftChildOf(parent))
            {
                var sibling = parent.Right;
                if (sibling == Nil)
                    break;

                if (sibling.IsRed)
                {
                    sibling.Color = Color.Black;
                    parent.Color = Color.Red;
                    // LeftRotate(parent);
                    sibling = parent.Right;
                }

                if (!sibling.Left.IsRed && !sibling.Right.IsRed)
                {
                    sibling.Color = Color.Red;
                    node = parent;
                }
                else
                {
                    if (!sibling.Right.IsRed)
                    {
                        sibling.Left.Color = Color.Black;
                        sibling.Color = Color.Red;
                        // RightRotate(sibling);
                        sibling = parent.Right;
                    }

                    sibling.Color = parent.Color;
                    parent.Color = Color.Black;
                    sibling.Right.Color = Color.Black;
                    // LeftRotate(parent);
                    node = Root;
                }
            }
            else
            {
                var sibling = parent.Left;
                if (sibling == Nil)
                    break;

                if (sibling.IsRed)
                {
                    sibling.Color = Color.Black;
                    parent.Color = Color.Red;
                    // RightRotate(parent);
                    sibling = parent.Left;
                }

                if (!sibling.Right.IsRed && !sibling.Left.IsRed)
                {
                    sibling.Color = Color.Red;
                    node = parent;
                }
                else
                {
                    if (!sibling.Left.IsRed)
                    {
                        sibling.Right.Color = Color.Black;
                        sibling.Color = Color.Red;
                        // LeftRotate(sibling);
                        sibling = parent.Left;
                    }

                    sibling.Color = parent.Color;
                    parent.Color = Color.Black;
                    sibling.Left.Color = Color.Black;
                    // RightRotate(parent);
                    node = Root;
                }
            }
        }

        node.Color = Color.Black;
    }
}
