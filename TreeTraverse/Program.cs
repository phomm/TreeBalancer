using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace TreeTraverse
{
    class Program
    {
        [DebuggerDisplay("{Node.Number}, R {Relations}")]
        private class NodeRelation
        {
            public int Relations { get; set; }
            public Node Node { get; }
            public NodeRelation(Node node, int relations)
            {
                Node = node;
                Relations = relations;
            }
        }

        [DebuggerDisplay("{Number}, S {SibsCount}")]
        private class Node
        {
            public int Number { get; set; }
            private readonly List<Node> mSiblings = new List<Node>();
            public IReadOnlyList<Node> Siblings { get => mSiblings; }
            public int SibsCount => Siblings.Count;

            public Node(int number)
            {
                Number = number;
            }
            
            public Node AddNode(Node node)
            {
                mSiblings.Add(node);
                return node;
            }

            public Node Search(int number, Node exclude = null)
            {
                if (Number == number)
                    return this;
                foreach(var sibling in Siblings.Except(new[] { exclude }))
                {
                    Node result = sibling.Search(number, this);
                    if (result != null)
                        return result;
                }
                return null;
            }

            public override string ToString()
            {
                return $"{Number}: {string.Join(",", Siblings.Select(n => n.Number))}";
            }
        }

        private static IEnumerable<Node> AddEdge(List<Node> tree, int from, int to)
        {
            
            Node fromNode = null;
            Node toNode = null;
            // different types of search and assign vars in single line (and preferably in single pass and in generic way and typo-errorprone)

            // kinda oneliner, but hard to read and not errorprone
            //foreach (var n in tree) _ = n.Number == from ? fromNode = n : n.Number == to ? toNode = n : null;

            /*
            // very good try, because errorprone and easy to read (and find mistake in assignment), but not oneliner and not single pass at all, up to O(N*N)
            var nodes = new [] { from, to }.ToDictionary(x => x.Key, x => tree.FirstOrDefault(n => n.Number == x.Key));
            Node fromNode = nodes[from];
            Node toNode = nodes[to];
            */

            /*
            // the one, with readability and ckecks for mistakenly written searching keys (dictionary internal dupe key check) , but not oneliner and not actualy single pass
            var dic = new Dictionary<int, Func<Node, Node>> { { from, n => fromNode = n }, { to, n => toNode = n } };
            tree.All(n => dic.TryGetValue(n.Number, out var func) ? func(n) is null : true);
            */        

            // best approach, BUT not generic (ofc one can write simple generic IEnumerable<T> ForEach extension method, and it would be strong candidate to win)
            //tree.ForEach(n => _ = n.Number == from ? fromNode = n : n.Number == to ? toNode = n : null);

            // nice approach, but not single pass, enumerating collection twice
            //tree.Zip(tree, (n, s) => n.Number == from ? fromNode = n : n.Number == to ? toNode = n : null);

            // the one I prefer best, however it's arguable due to breaking functional approach of Linq, causing side effects
            tree.All(n => (n.Number == from ? fromNode = n : n.Number == to ? toNode = n : null) is null);

            var result = new List<Node>();
            Node genNode(int num)
            {
                var node = new Node(num);
                result.Add(node);
                return node;
            }
            if (toNode == null || fromNode == null)
            {
                fromNode = fromNode ?? genNode(from);
                fromNode.AddNode(toNode ?? genNode(to)).AddNode(fromNode);
            }
            return result;
        }

        private static List<Node> GenerateTree()
        {
            var result = new List<Node>();
            result.AddRange(AddEdge(result, 1, 4));
            result.AddRange(AddEdge(result, 2, 4));
            result.AddRange(AddEdge(result, 3, 4));
            result.AddRange(AddEdge(result, 4, 5));
            result.AddRange(AddEdge(result, 5, 6));
            result.AddRange(AddEdge(result, 6, 7));
            result.AddRange(AddEdge(result, 7, 8));
            result.AddRange(AddEdge(result, 7, 9));
            result.AddRange(AddEdge(result, 6, 10));
            result.AddRange(AddEdge(result, 10, 11));
            result.AddRange(AddEdge(result, 11, 12));
            result.AddRange(AddEdge(result, 11, 13));
            result.AddRange(AddEdge(result, 12, 14));
            result.AddRange(AddEdge(result, 13, 15));
            result.AddRange(AddEdge(result, 13, 16));

            return result;
        }

        private static Node getCentralNode(List<Node> tree)
        {
            var nodeQueue = new Queue<NodeRelation>(tree.Where(node => node.SibsCount == 1)
                .Select(node => new NodeRelation(node, node.SibsCount)));
            var removedNodes = new List<Node>();
            while (nodeQueue.Count > 1)
            {
                var relation = nodeQueue.Dequeue();
                if (relation.Relations > 1)
                {
                    nodeQueue.Enqueue(relation);
                }
                else if (relation.Relations == 1)
                {
                    var nodes = relation.Node.Siblings.Except(removedNodes);
                    removedNodes.Add(relation.Node);
                    var count = nodes.Count();
                    foreach (var node in nodes)
                    {
                        if (nodeQueue.LastOrDefault(nr => nr.Node == node) is NodeRelation nodeRelation)
                        {
                            nodeRelation.Relations--;
                        }
                        else
                        {
                            nodeQueue.Enqueue(new NodeRelation(node, node.SibsCount - 1));
                        }
                    }
                }
            }
            return nodeQueue.First().Node;
        }

        static void Main(string[] args)
        {            
            var tree = GenerateTree();
            foreach (var node in tree.Where(n => n != null))
            {
                Console.WriteLine(node);
            }

            Console.WriteLine($"Center: {getCentralNode(tree).Number}");
            Console.ReadLine();                
        }
    }
}
