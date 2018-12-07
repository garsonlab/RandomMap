using System.Collections.Generic;
using UnityEngine;

namespace TinyGrid
{
    public class TinyGenegrater : MonoBehaviour
    {
        public int width = 5;
        public int height = 4;
        [Space]
        public int roomCount = 5;
        public int fieldCount = 8;
        public int roomDensity = 3;
        public int fieldDensity = 3;
        public int maxChance = 0;

        public List<Color> colors = new List<Color>()
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

        public GameObject tiled;
        private int step = 0;

        void Start()
        {
            CreateGrids();

            while (true)
            {
                step = 0;
                ResetGrids();
                CreateRooms();
                PrintTex(step++.ToString());
                CreateFields();
                PrintTex(step++.ToString());
                ConnectRoomField();

                if(ConnectFields())
                    break;
            }
            
        }

        void CreateGrids()
        {
            grids = new MGrid[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int2 pos = new int2(i, j);
                    var obj = GameObject.Instantiate(tiled) as GameObject;
                    var grid = obj.AddComponent<MGrid>();
                    grid.Init(pos);
                    grids[i, j] = grid;
                }
            }
            finder.UpdateNodes(grids);
        }
        
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
                        
                        MGrid.SetBorder((MGrid)pre, (MGrid)node);
                        pre = node;
                    }

                    MGrid field = (MGrid) path[0];
                    fieldFlag.Add(field);


                    PrintTex(step++.ToString());
                }
            }

            foreach (var field in fields)
            {
                if (!fieldFlag.Contains(field))
                {
                    field.GridType = GridType.Empty;
                }
            }

            PrintTex(step++.ToString());
            fields = fieldFlag;
        }

        bool ConnectFields()
        {
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    bool isFound = finder.Search(fields[i], fields[j], SearchLayer.PathAndField);

                    if (!isFound)
                        isFound = finder.Search(fields[i], fields[j], SearchLayer.PathFieldAndEmpty);

                    if (!isFound)
                        return false;
                    var path = finder.GetPath();

                    INode pre = null;
                    foreach (var grid in path)
                    {
                        if (grid.GridType == GridType.Empty)
                            grid.GridType = GridType.Path;

                        if (pre != null)
                            MGrid.SetBorder((MGrid)pre, (MGrid)grid);
                        pre = grid;
                    }
                    PrintTex(step++.ToString());
                }
            }

            return true;
        }

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


        void PrintTex(string n)
        {
            Texture2D texture = new Texture2D(width * 3, height * 3);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var grid = grids[i, j];
                    int x = i * 3 + 1;
                    int y = ((j+height-1)%height) * 3 + 1;

                    Color c = colors[(int)grid.GridType];
                    texture.SetPixel(x, y, c);
                    if (grid.connector.IsConnected(Connector.ConnectDir.Right))
                        texture.SetPixel(x + 1, y, Color.blue);
                    if (grid.connector.IsConnected(Connector.ConnectDir.Up))
                        texture.SetPixel(x, y + 1, Color.blue);
                    if (grid.connector.IsConnected(Connector.ConnectDir.Left))
                        texture.SetPixel(x - 1, y, Color.blue);
                    if (grid.connector.IsConnected(Connector.ConnectDir.Down))
                        texture.SetPixel(x, y - 1, Color.blue);
                }
            }

            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes("F:/cc/" + n + ".png", bytes);
        }

    }
}