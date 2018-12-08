using System;

namespace TinyGrid
{
    [System.Serializable]
    public struct Point : IEquatable<Point>
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point up => new Point(0, 1);
        public static Point down => new Point(0, -1);
        public static Point left => new Point(-1, 0);
        public static Point right => new Point(1, 0);
        public static Point one => new Point(1, 1);
        public static Point zero => new Point(0, 0);
        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
        public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);

        public static implicit operator UnityEngine.Vector2(Point pos) => new UnityEngine.Vector2(pos.x, pos.y);
        public static implicit operator UnityEngine.Vector3(Point pos) => new UnityEngine.Vector3(pos.x, pos.y);

        public int AbsLen => Math.Abs(x) + Math.Abs(y);

        bool IEquatable<Point>.Equals(Point other) => this.x == other.x && this.y == other.y;

        public override string ToString() => $"({x},{y})";
    }
}
