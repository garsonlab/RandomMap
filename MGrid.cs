using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinyGrid
{
    public interface INode
    {
        /// <summary>当前位置</summary>
        Point Position { get; }
        /// <summary>格子类型</summary>
        GridType GridType { get; set; }
        /// <summary>
        /// 是否在搜索层
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        bool InSearchLayer(SearchLayer layer);

        void SetNeighbor(INode other);
    }

    public enum GridType
    {
        /// <summary>空</summary>
        Empty = 0,
        /// <summary>房间</summary>
        Room = 1,
        /// <summary>野外点</summary>
        Field = 2,
        /// <summary>路径点</summary>
        Path = 3,
    }

    public enum SearchLayer
    {
        /// <summary>连接房间和野外只走空</summary>
        ConnectRoomField = 0,
        /// <summary>使用野外和路</summary>
        PathAndField = 1,
        /// <summary>使用野外、路、空</summary>
        PathFieldAndEmpty = 2,
    }

    public enum Direction
    {
        Right = 0,
        Up = 1,
        Left = 2,
        Down = 3
    }

    public class MGrid : MonoBehaviour, INode
    {
        [SerializeField]
        private Point position;
        [SerializeField]
        private GridType gType;

        private Dictionary<Direction, INode> dirGrids;

        public Point Position => position;

        public GridType GridType
        {
            get => gType;
            set => gType = value;
        }

        public void Init(Point pos)
        {
            position = pos;
            transform.position = pos;
            dirGrids = new Dictionary<Direction, INode>();
            Clear();
        }
        
        public bool InSearchLayer(SearchLayer layer)
        {
            switch (layer)
            {
                case SearchLayer.ConnectRoomField:
                case SearchLayer.PathFieldAndEmpty:
                    return gType == GridType.Empty || gType == GridType.Path || gType == GridType.Field;
                case SearchLayer.PathAndField:
                    return gType == GridType.Path || gType == GridType.Field;
            }

            return false;
        }

        public void Clear()
        {
            gType = GridType.Empty;
            dirGrids.Clear();
        }

        public void SetNeighbor(INode other)
        {
            if(other == null)
                return;

            Point offset = other.Position - position;
            if (offset.AbsLen != 1)
                throw new Exception($"{this} Is Not Close To {other}");

            if (offset.x > 0)
                dirGrids[Direction.Right] = other;
            else if (offset.y > 0)
                dirGrids[Direction.Up] = other;
            else if (offset.x < 0)
                dirGrids[Direction.Left] = other;
            else if (offset.y < 0)
                dirGrids[Direction.Down] = other;
        }

        public override string ToString()
        {
            return $"{gType}{position}";
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = TinyGenegrater.colors[(int) gType];
            Gizmos.DrawCube(transform.position, Vector3.one*0.8f);

            Gizmos.color = Color.blue;
            foreach (var dir in dirGrids)
            {
                switch (dir.Key)
                {
                    case Direction.Right:
                        Gizmos.DrawCube(transform.position + new Vector3(0.45f, 0), new Vector3(0.1f, 0.8f, 1));
                        break;
                    case Direction.Up:
                        Gizmos.DrawCube(transform.position + new Vector3(0, 0.45f, 0), new Vector3(0.8f, 0.1f, 1));
                        break;

                    case Direction.Left:
                        Gizmos.DrawCube(transform.position - new Vector3(0.45f, 0), new Vector3(0.1f, 0.8f, 1));
                        break;
                    case Direction.Down:
                        Gizmos.DrawCube(transform.position - new Vector3(0, 0.45f, 0), new Vector3(0.8f, 0.1f, 1));
                        break;
                }
            }
        }

    }
}