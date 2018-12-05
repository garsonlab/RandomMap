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

        public readonly List<NodeWrap<T>> elements = new List<NodeWrap<T>>();

        public int Count
        {
            get { return elements.Count; }
        }

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


    public class AStarFinder<T>
    {
        private int width, height;
        private T[,] nodes;

        public void UpdateNodes(T[,] nodes)
        {
            this.width = nodes.GetLength(0);
            this.height = nodes.GetLength(1);
            this.nodes = nodes;
        }
        
        public bool Contains(int2 pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        //public T GetNode(int2 pos)
        //{
        //    if (Contains(pos))
        //        return nodes[pos.x, pos.y];
        //    return null;
        //}

    }
}
