using System.Collections.Generic;

namespace TinyGrid
{
    public class NodeWrap<T>
    {
        public T node;
        public int priority;

        public NodeWrap(T node, int priority)
        {
            this.node = node;
            this.priority = priority;
        }
    }

    public class PriorityQueue<T>
    {
        private readonly List<NodeWrap<T>> elements = new List<NodeWrap<T>>();

        public int Count => elements.Count;

        public void Enqueue(T node, int priority)
        {
            elements.Add(new NodeWrap<T>(node, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].priority < elements[bestIndex].priority)
                {
                    bestIndex = i;
                }
            }

            var bestItem = elements[bestIndex];
            elements.RemoveAt(bestIndex);
            return bestItem.node;
        }
        
        public void Clear()
        {
            elements.Clear();
        }
    }

    public class SquareRectFinder
    {
        private int width, height;
        private INode[,] nodes;
        private PriorityQueue<INode> frontier = new PriorityQueue<INode>();
        private Dictionary<INode, INode> cameFrom = new Dictionary<INode, INode>();
        private Dictionary<INode, int> cost = new Dictionary<INode, int>();
        private List<Point> dirs = new List<Point>() { Point.right, Point.up, Point.left, Point.down };
        private List<INode> path = new List<INode>();

        public void UpdateNodes(INode[,] nodes)
        {
            this.width = nodes.GetLength(0);
            this.height = nodes.GetLength(1);
            this.nodes = nodes;
        }
        
        public bool Contains(Point pos) => pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;

        public INode GetNode(Point pos) => Contains(pos) ? nodes[pos.x, pos.y] : null;

        public List<INode> GetPath() => path;

        public bool Search(INode start, INode goal, SearchLayer layer)
        {
            cameFrom.Clear();
            cost.Clear();
            frontier.Clear();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            cost[start] = 0;
            bool isFound = false;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current == goal)
                {
                    isFound = true;
                    break;
                }

                foreach (var neighbor in GetNeighbors(current, layer))
                {
                    int newCost = cost[current] + 1;//所有的代价都为1
                    if (!cost.TryGetValue(neighbor, out int nearCost) || newCost < nearCost)
                    {
                        cost[neighbor] = nearCost;

                        int priority = newCost + (neighbor.Position - goal.Position).AbsLen;
                        frontier.Enqueue(neighbor, priority);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            path.Clear();
            if (isFound)
            {
                INode cur = goal;
                while (!cur.Equals(start))
                {
                    path.Add(cur);
                    if (!cameFrom.TryGetValue(cur, out cur))
                        break;
                }
                path.Add(start);
            }

            return isFound;
        }
        
        private IEnumerable<INode> GetNeighbors(INode current, SearchLayer layer)
        {
            //随机方向？
            foreach (var dir in dirs)
            {
                Point pos = current.Position + dir;
                var grid = GetNode(pos);
                if (grid != null && grid.InSearchLayer(layer))
                    yield return grid;
            }
        }

    }
}
