using System;

namespace TinyGrid
{
    [System.Serializable]
    public struct int2 : IEquatable<int2>
    {
        public int x;
        public int y;

        public int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static int2 up { get { return new int2(0, 1); } }
        public static int2 down { get { return new int2(0, -1); } }
        public static int2 left { get { return new int2(-1, 0); } }
        public static int2 right { get { return new int2(1, 0); } }
        public static int2 one { get { return new int2(1, 1); } }
        public static int2 zero { get { return new int2(0, 0); } }
        public static int2 operator +(int2 a, int2 b)
        {
            return new int2(a.x + b.x, a.y + b.y);
        }
        public static int2 operator -(int2 a, int2 b)
        {
            return new int2(a.x - b.x, a.y - b.y);
        }

        public int AbsLen
        {
            get
            {
                return Math.Abs(x) + Math.Abs(y);
            }
        }

        bool IEquatable<int2>.Equals(int2 other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
    }
}
