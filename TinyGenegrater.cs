using System.Collections.Generic;
using UnityEngine;

namespace TinyGrid
{
    public class TinyGenegrater : MonoBehaviour
    {
        public int width = 5;
        public int height = 4;
        [Space]
        public int size = 4;
        public int roomCount = 5;
        public int fieldCount = 8;
        public int roomDensity = 3;
        public int fieldDensity = 3;
        public int maxChance = 10;

        public static readonly List<Color> colors = new List<Color>()
        {
            Color.white,
            Color.red,
            Color.green,
            Color.yellow
        };

        private MGrid[,] grids;
        private List<MGrid> rooms = new List<MGrid>();
        private List<MGrid> fields = new List<MGrid>();
        private SquareRectFinder finder = new SquareRectFinder();
        

        void Start()
        {
            Create();
        }

        public void Create()
        {
            CreateGrids();

            while (true)
            {
                ResetGrids();
                CreateRooms();
                CreateFields();
                ConnectRoomField();

                if (ConnectFields())
                    break;
            }

            SlipGrid();
        }


        #region Private Methods
        /// <summary>
        /// 创建格子
        /// </summary>
        void CreateGrids()
        {
            grids = new MGrid[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Point pos = new Point(i, j);
                    var obj = new GameObject(pos.ToString());
                    var grid = obj.AddComponent<MGrid>();
                    grid.Init(pos);
                    grids[i, j] = grid;
                }
            }
            finder.UpdateNodes(grids);
        }
        
        /// <summary>
        /// 创建房间
        /// </summary>
        void CreateRooms()
        {
            rooms.Clear();
            for (int i = 0; i < roomCount; i++)
            {
                var grid = RandomGrid(rooms, roomDensity);
                if (grid == null)
                    break;

                grid.GridType = GridType.Room;
                rooms.Add(grid);
            }
        }

        /// <summary>
        /// 创建中间点
        /// </summary>
        void CreateFields()
        {
            fields.Clear();
            for (int i = 0; i < fieldCount; i++)
            {
                var grid = RandomGrid(fields, fieldDensity);
                if (grid == null)
                    break;

                grid.GridType = GridType.Field;
                fields.Add(grid);
            }
        }

        /// <summary>
        /// 连接房间和中间点
        /// </summary>
        void ConnectRoomField()
        {
            List<MGrid> fieldFlag = new List<MGrid>();
            int minStep;
            INode[] path;
            foreach (var room in rooms)
            {
                minStep = int.MaxValue;
                path = null;
                foreach (var field in fields)
                {
                    if (finder.Search(room, field, SearchLayer.PathFieldAndEmpty))
                    {
                        var list = finder.GetPath();
                        if (list.Count < minStep)
                        {
                            minStep = list.Count;
                            path = list.ToArray();
                        }
                    }
                }

                if (path != null)
                {
                    INode pre = null;
                    foreach (var node in path)
                    {
                        if (node.GridType == GridType.Empty)
                            node.GridType = GridType.Path;

                        if (pre != null)
                        {
                            node.SetNeighbor(pre);
                            pre.SetNeighbor(node);
                        }
                        pre = node;
                    }

                    MGrid field = (MGrid) path[0];
                    fieldFlag.Add(field);
                }
            }

            foreach (var field in fields)
            {
                if (!fieldFlag.Contains(field))
                {
                    field.GridType = GridType.Empty;
                }
            }

            fields = fieldFlag;
        }

        /// <summary>
        /// 连接中间点
        /// </summary>
        /// <returns></returns>
        bool ConnectFields()
        {
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    //优先经过已存在道路
                    bool isFound = finder.Search(fields[i], fields[j], SearchLayer.PathAndField);

                    if (!isFound)//搜索不到再附带空地，减少多余格子
                        isFound = finder.Search(fields[i], fields[j], SearchLayer.PathFieldAndEmpty);

                    if (!isFound)//表示找不到一条不经过Room连接两个野外的道路
                        return false;
                    var path = finder.GetPath();

                    INode pre = null;
                    foreach (var node in path)
                    {
                        if (node.GridType == GridType.Empty)
                            node.GridType = GridType.Path;

                        if (pre != null)
                        {
                            node.SetNeighbor(pre);
                            pre.SetNeighbor(node);
                        }
                        pre = node;
                    }

                }
            }

            return true;
        }

        /// <summary>
        /// 重置格子
        /// </summary>
        void ResetGrids()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grids[i, j].Clear();
                }
            }
        }

        /// <summary>
        /// 随机一个空格子，与其他的格子保持距离在density以上
        /// </summary>
        /// <param name="list">其他格子j</param>
        /// <param name="density">距离</param>
        /// <returns></returns>
        MGrid RandomGrid(List<MGrid> list, int density)
        {
            int chance = maxChance;

            MGrid defGrid = null;

            while (chance > 0)
            {
                --chance;

                int x = Random.Range(0, width);
                int y = Random.Range(0, height);

                var grid = grids[x, y];
                if (grid.GridType != GridType.Empty)//当前已是其他类型
                    continue;

                if (defGrid == null)
                    defGrid = grid;

                bool inBounds = false;//是否在范围内
                foreach (var item in list)
                {
                    if ((item.Position - grid.Position).AbsLen < density)
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
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (grids[i, j].GridType == GridType.Empty)
                        {
                            defGrid = grids[i, j];
                            break;
                        }
                    }
                }
            }

            return defGrid;
        }
        
        #endregion


        void SlipGrid()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    
                }
            }
        }
    }
}