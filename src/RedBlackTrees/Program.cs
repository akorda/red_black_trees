namespace RedBlackTrees.Paper;

// see: https://www.geeksforgeeks.org/dsa/introduction-to-red-black-tree/

public static class Program
{
    public static void Main()
    {
        var tree = new RedBlackTree<int, string>();

        // var keys = new int[] { 11, 2, 14, 1, 7, 15, 5, 8, 4 };
        // var keys = new int[] { 14, 11, 2, 3 };
        // keys for paper "A dichromatic framework for balanced trees"
        // var keys = new int[] { 1, 9, 2, 8, 3, 7, 4, 6, 5 };
        var random = Random.Shared;
        var keys = new List<int>();
        Enumerable.Range(0, 1000).ToList().ForEach(i => keys.Add(random.Next(1000)));

        var writer = new ConsoleTreeWriter<int, string>();
        foreach (var key in keys)
        {
            var value = $"{key}";
            Console.WriteLine($"Insert {key}");
            tree.Insert(key, value);
        }


        writer.Write(tree, Console.Out);
        // new LatexTreeWriter<int, string>().Write(tree, Console.Out);
        // new LatexSameLevelTreeWriter<int, string>().Write(tree, Console.Out);
    }
}
