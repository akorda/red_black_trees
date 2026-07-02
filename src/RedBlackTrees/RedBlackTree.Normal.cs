namespace RedBlackTrees.Normal;

public enum Color : byte
{
    Red,
    Black,
}

public class Node<K, V>
{
    public K Key { get; set; }
    public V Value { get; set; }
    public Color Color { get; set; }
    public Node<K, V> Left { get; set; }
    public Node<K, V> Right { get; set; }
    public Node<K, V> Parent { get; set; }

    public Node(K key, V value, Node<K, V> nil)
    {
        Key = key;
        Value = value;
        Color = Color.Red;
        Left = nil;
        Right = nil;
        Parent = nil;
    }

    public static Node<K, V> CreateNil()
        => new(default!, default!, null!) { Color = Color.Black };

    public bool IsLeftChild => Parent?.Left == this;
    public bool IsRightChild => !IsLeftChild;
    public bool IsRed => Color == Color.Red;
    public bool IsBlack => Color == Color.Black;
}


public class RedBlackTree<K, V>
    where K : IComparable
{

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

        while (x != Nil)
        {
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

        var z = new Node<K, V>(key, value, Nil)
        {
            Parent = y
        };

        if (y == Nil) Root = z;
        else if (compare < 0) y.Left = z;
        else y.Right = z;

        InsertFixup(z);

        Size++;
    }

    private void InsertFixup(Node<K, V> z)
    {
        while (z.Parent.IsRed)
        {
            if (z.Parent.IsLeftChild)
            {
                var uncle = z.Parent.Parent.Right;

                if (uncle.IsRed)
                {
                    z.Parent.Color = Color.Black;
                    uncle.Color = Color.Black;
                    z.Parent.Parent.Color = Color.Red;
                    z = z.Parent.Parent;
                }
                else
                {
                    if (z.IsRightChild)
                    {
                        z = z.Parent;
                        LeftRotate(z);
                    }
                    z.Parent.Color = Color.Black;
                    z.Parent.Parent.Color = Color.Red;
                    RightRotate(z.Parent.Parent);
                }
            }
            else
            {
                var uncle = z.Parent.Parent.Left;

                if (uncle.IsRed)
                {
                    z.Parent.Color = Color.Black;
                    uncle.Color = Color.Black;
                    z.Parent.Parent.Color = Color.Red;
                    z = z.Parent.Parent;
                }
                else
                {
                    if (z.IsLeftChild)
                    {
                        z = z.Parent;
                        RightRotate(z);
                    }
                    z.Parent.Color = Color.Black;
                    z.Parent.Parent.Color = Color.Red;
                    LeftRotate(z.Parent.Parent);
                }
            }
        }

        Root!.Color = Color.Black;
    }

    private void RightRotate(Node<K, V> x)
    {
        var y = x.Left;

        x.Left = y.Right;

        if (y.Right != Nil)
        {
            y.Right.Parent = x;
        }

        y.Parent = x.Parent;

        if (x.Parent == Nil)
        {
            // we are the root
            Root = y;
        }
        else if (x.IsRightChild)
        {
            x.Parent.Right = y;
        }
        else
        {
            x.Parent.Left = y;
        }

        y.Right = x;
        x.Parent = y;
    }

    private void LeftRotate(Node<K, V> x)
    {
        var y = x.Right;

        x.Right = y.Left;

        if (y.Left != Nil)
        {
            y.Left.Parent = x;
        }

        y.Parent = x.Parent;

        if (x.Parent == Nil)
        {
            // we are the root
            Root = y;
        }
        else if (x.IsLeftChild)
        {
            x.Parent.Left = y;
        }
        else
        {
            x.Parent.Right = y;
        }

        y.Left = x;
        x.Parent = y;
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
