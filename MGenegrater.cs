using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tiny
{
    public class MGenegrater : MonoBehaviour
    {
        public int width = 5;
        public int height = 4;
        public Grid[,] grids;

        public int roomCount = 5;
        public int fieldCount = 8;

        public int roomDensity = 3;
        public int fieldDensity = 3;
        public int maxChance = 10;

        public List<Color> colors = new List<Color>()
        {
            Color.white,
            Color.red,
            Color.green,
            Color.yellow
        };


        List<Grid> rooms = new List<Grid>();
        List<Grid> fields = new List<Grid>();
        private SquareRect square = new SquareRect();
        private bool canDraw = false;

        private int ss = 4;

        void Start()
        {
            float t = Time.realtimeSinceStartup;
            CreateGrids();

            //while (true)
            //{
            //    if(Genegrate())
            //        break;
            //    Debug.Log("生成失败，重置");
            //}

            ////PrintTex("4");
            //Debug.Log("@@@@@\t" + (Time.realtimeSinceStartup-t));
            //PrintGrids();
            //canDraw = true;
        }

        void OnDrawGizmos()
        {
            if(!canDraw)
                return;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var grid = grids[i, j];
                    Gizmos.color = colors[(int) grid.gType];
                    Gizmos.DrawCube(new Vector3(grid.position.y, 0, grid.position.x), Vector3.one*0.9f);

                    //if (grid.parent != null)
                    //{
                    //    Gizmos.color = Color.blue;
                    //    //Gizmos.DrawLine(new Vector3(grid.parent.position.x, 2, grid.parent.position.y), new Vector3(grid.position.x, 2, grid.position.y));

                    //    int2 p = grid.parent.position + grid.position;
                    //    int2 s = grid.parent.position - grid.position;

                    //    Vector3 pos = new Vector3(p.x * 0.5f, 2, p.y * 0.5f);
                    //    Vector3 sca = new Vector3(Math.Abs(s.x) + 1, 1, Math.Abs(s.y) + 1);

                    //    Gizmos.DrawCube(pos, sca);
                    //}
                }
            }
        }


        public GameObject prefab;
        public int step = 0;
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                
            }
        }


        void PrintGrids()
        {
            for (int i = 0; i < height; i++)
            {
                string line = "";
                for (int j = 0; j < width; j++)
                {
                    //int v = (int)grids[j, i].gType;

                    line += grids[j, i].ToString() + ", ";
                }
                Debug.Log(line);
            }
        }
        

        bool Genegrate()
        {
            ss = 4;
            ResetGrids();
            CreateRooms();
            PrintTex("1");
            CreateFields();
            PrintTex("2");
            square.UpdateGrids(grids);
            ConnectRoomField();
            return ConnectFieldPath();
        }

        /// <summary>创建格子</summary>
        void CreateGrids()
        {
            grids = new Grid[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grids[i, j] = new Grid(new int2(i, j));
                }
            }
        }
        /// <summary>初始化格子</summary>
        void ResetGrids()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grids[i, j].Reset();
                }
            }
        }
        /// <summary>随机房间</summary>
        void CreateRooms()
        {
            for (int i = 0; i < roomCount; i++)
            {
                var grid = RandomGrid(1, roomDensity, rooms);
                if (grid == null)
                    break;

                grid.gType = GridType.Room;
                rooms.Add(grid);
            }
        }
        /// <summary>随机野外点</summary>
        void CreateFields()
        {
            for (int i = 0; i < fieldCount; i++)
            {
                var grid = RandomGrid(2, fieldDensity, fields);
                if (grid == null)
                    break;

                grid.gType = GridType.Field;
                fields.Add(grid);
            }
        }
        /// <summary>连接房间和最近的野外的，清除多余的野外点</summary>
        void ConnectRoomField()
        {
            //Debug.Log("开始连接房间与野外");
            List<Grid> fieldFlag = new List<Grid>();
            int minStep;
            Grid[] path;
            foreach (var room in rooms)
            {
                minStep = int.MaxValue;
                path = null;

                foreach (var field in fields)
                {
                    bool isFound = square.Search(room, field, SearchLayer.ConnectRoomField);
                    //Debug.Log("测试 房间 " + room.position + " 野外" + field.position + " 结果 " + isFound);

                    if (isFound)
                    {
                        List<Grid> gs = square.GetPath();
                        if (gs.Count < minStep)
                        {
                            //Debug.Log("选择当前 " + minStep);
                            path = gs.ToArray();
                            minStep = gs.Count;
                        }
                    }
                }

                if (path != null)
                {
                    //string a = "";
                    Grid pre = null;
                    foreach (var grid in path)
                    {
                        //Debug.Log(grid.position + " 设置路径");
                        if (grid.gType == 0)
                            grid.gType = GridType.Path;

                        //if (pre != null && grid.parent == null)
                        //    grid.parent = pre;
                        //a += grid.position + "  ";
                        if (pre != null)
                        {
                            Grid.SetBorder(pre, grid);
                        }
                        pre = grid;
                    }

                    fieldFlag.Add(path[0]);

                    //Debug.Log("连接 " + path[0] + "       " + path[path.Length-1]);
                    //Debug.Log(a);
                }

                PrintTex(ss.ToString());
                ss++;
            }


            foreach (var field in fields)
            {
                if (!fieldFlag.Contains(field))
                {
                    //Debug.Log(field.position + "设置空");
                    field.gType = GridType.Empty;
                }
            }
            PrintTex(ss.ToString());
            ss++;
            fields = fieldFlag;
        }
        /// <summary>连接野外点和路</summary>
        bool ConnectFieldPath()
        {
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = i+1; j < count; j++)
                {
                    bool isFound = square.Search(fields[i], fields[j], SearchLayer.PathAndField);

                    if (!isFound)
                        isFound = square.Search(fields[i], fields[j], SearchLayer.PathFieldAndEmpty);

                    if (!isFound)
                        return false;
                    var path = square.GetPath();
                    Grid pre = null;
                    foreach (var grid in path)
                    {
                        if (grid.gType == GridType.Empty)
                            grid.gType = GridType.Path;

                        if (pre != null)
                        {
                            Grid.SetBorder(pre, grid);
                        }
                        pre = grid;
                    }

                    PrintTex(ss.ToString());
                    ss++;
                }
            }

            return true;
        }
        /// <summary>随机格子</summary>
        Grid RandomGrid(int type, int density, List<Grid> list)
        {
            int chance = maxChance;

            Grid defGrid = null;

            while (chance > 0)
            {
                --chance;

                int x = Random.Range(0, width);
                int y = Random.Range(0, height);

                var grid = grids[x, y];
                if (grid.gType != 0 || list.Contains(grid))
                    continue;

                if (defGrid == null)
                    defGrid = grid;

                bool inBounds = false;
                foreach (var item in list)
                {
                    if ((item.position - grid.position).AbsLen < density)
                    {
                        inBounds = true;
                        break;
                    }
                }

                if (!inBounds)
                    return grid;
            }

            if (defGrid == null)
            {
                Debug.Log("悲催的没有随机到");
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (grids[i, j].gType == 0)
                            return defGrid;
                    }
                }
            }

            return defGrid;
        }


        void PrintTex(string n)
        {
            Texture2D texture = new Texture2D(width * 3, height*3);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var grid = grids[i, j];
                    int x = i * 3 + 1;
                    int y = j * 3 + 1;

                    Color c = colors[(int) grid.gType];
                    texture.SetPixel(x, y, c);
                    if(grid.connector.right != null)
                        texture.SetPixel(x+1, y, Color.blue);
                    if (grid.connector.up != null)
                        texture.SetPixel(x, y + 1, Color.blue);
                    if (grid.connector.left != null)
                        texture.SetPixel(x - 1, y, Color.blue);
                    if (grid.connector.down != null)
                        texture.SetPixel(x, y-1, Color.blue);
                }
            }

            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes("F:/cc/" + n+".png", bytes);
        }


    }
















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
        public static int2 down { get { return  new int2(0,-1);} }
        public static int2 left { get { return new int2(-1, 0); } }
        public static int2 right { get { return new int2(1, 0); } }
        public static int2 one { get { return new int2(1, 1); } }
        public static int2 zero { get { return new int2(0, 0); } }
        public static int2 operator +(int2 a, int2 b)
        {
            return new int2(a.x+b.x, a.y+b.y);
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

    public enum GridType
    {
        Empty = 0,
        Room = 1,
        Field = 2,
        Path = 3,
    }

    public enum SearchLayer
    {
        /// <summary>连接room-field</summary>
        ConnectRoomField = 0,
        /// <summary>使用野外和路</summary>
        PathAndField = 1,
        /// <summary>使用野外、路、空</summary>
        PathFieldAndEmpty = 2,
    }

    public class Grid : MonoBehaviour, IEquatable<Grid>
    {
        public readonly int2 position;
        public GridType gType;
        public Grid parent;
        public Connector connector;
        

        public Grid(int2 pos)
        {
            position = pos;
            gType = GridType.Empty;
        }

        bool IEquatable<Grid>.Equals(Grid other)
        {
            return this == other;
        }

        public bool InSearchLayer(SearchLayer layer)
        {
            switch (layer)
            {
                case SearchLayer.PathFieldAndEmpty:
                case SearchLayer.ConnectRoomField:
                    return gType == GridType.Empty || gType == GridType.Path || gType == GridType.Field;
                case SearchLayer.PathAndField:
                    return gType == GridType.Path || gType == GridType.Field;
            }

            return false;
        }

        public void Reset()
        {
            gType = GridType.Empty;
            parent = null;
            connector.Clear();
        }
        

        public static void SetBorder(Grid a, Grid b)
        {
            int2 offset = a.position - b.position;

            if(offset.AbsLen != 1 )
                throw new Exception(a.position + " is not close to " + b.position);
            if (offset.x > 0)
            {
                b.connector.right = a;
                a.connector.left = b;
            }
            else if (offset.y > 0)
            {
                b.connector.up = a;
                a.connector.down = b;
            }
            else if (offset.x < 1)
            {
                b.connector.left = a;
                a.connector.right = b;
            }
            else if (offset.y < 0)
            {
                b.connector.down = a;
                a.connector.up = b;
            }
            //Debug.Log("连接" + a.position + "  " + b.position + "  " + a.connector + "  " + b.connector);
        }

        public override string ToString()
        {
            return (int) gType + " " + position + connector;
        }
    }


    public struct Connector
    {
        public Grid right;
        public Grid up;
        public Grid left;
        public Grid down;

        public void Clear()
        {
            right = null;
            up = null;
            left = null;
            down = null;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", right != null, up != null, left != null, down != null);
        }
    }

    public class GridWrap
    {
        public Grid grid;
        public int priority;

        public GridWrap(Grid grid, int priority)
        {
            this.grid = grid;
            this.priority = priority;
        }
    }
    
    public class SquareRect
    {
        private int width, height;
        private Grid[,] grids; 
        private PriorityQueue frontier = new PriorityQueue();
        private Dictionary<Grid, Grid> cameFrom = new Dictionary<Grid, Grid>();
        private Dictionary<Grid, int> cost = new Dictionary<Grid, int>();
        private List<int2> dirs = new List<int2>(){int2.right, int2.up, int2.left, int2.down};
        private List<Grid> path = new List<Grid>();

        public void UpdateGrids(Grid[,] grids)
        {
            this.width = grids.GetLength(0);
            this.height = grids.GetLength(1);
            this.grids = grids;
        }
        
        public bool Contains(int2 pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        public Grid GetGrid(int2 pos)
        {
            if (Contains(pos))
                return grids[pos.x, pos.y];
            return null;
        }

        public List<Grid> GetPath()
        {
            return path;
        }

        public bool Search(Grid start, Grid goal, SearchLayer layer)
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

                foreach (var nerghbor in GetNeighbors(current, layer))
                {
                    int newCost = cost[current] + 1;//所有的代价都为1
                    int nearCost;
                    if (!cost.TryGetValue(nerghbor, out nearCost) || newCost < nearCost)
                    {
                        cost[nerghbor] = nearCost;

                        int priority = newCost + (nerghbor.position - goal.position).AbsLen;
                        frontier.Enqueue(nerghbor, priority);
                        cameFrom[nerghbor] = current;
                    }
                }
            }

            path.Clear();
            if (isFound)
            {
                Grid cur = goal;
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
        
        private IEnumerable<Grid> GetNeighbors(Grid current, SearchLayer layer)
        {
            //随机方向？
            foreach (var dir in dirs)
            {
                int2 pos = current.position + dir;
                var grid = GetGrid(pos);
                if (grid != null && grid.InSearchLayer(layer))
                    yield return grid;
            }
        }

        public void Clear()
        {
            grids = null;
            frontier.Clear();
            cameFrom.Clear();
            cost.Clear();
        }
    }

    public class PriorityQueue
    {
        public readonly List<GridWrap> elements = new List<GridWrap>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(Grid grid, int priority)
        {
            elements.Add(new GridWrap(grid, priority));
        }

        public Grid Dequeue()
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
            return bestItem.grid;
        }

        public void Clear()
        {
            elements.Clear();
        }
    }

    
}