using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyGrid
{
    public interface INode
    {
        int2 Position { get; }
        GridType GridType { get; set; }
        bool InSearchLayer(SearchLayer layer);
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
        ConnectRoomField = 0,
        /// <summary>使用野外和路</summary>
        PathAndField = 1,
        /// <summary>使用野外、路、空</summary>
        PathFieldAndEmpty = 2,
    }
    [System.Serializable]
    public class Connector
    {
        public enum ConnectDir
        {
            Right = 0,
            Up = 1,
            Left = 2,
            Down = 3
        }

        public MGrid[] neighbors = new MGrid[4];

        public bool IsConnected(ConnectDir dir)
        {
            return neighbors[(int)dir] != null;
        }

        public void SetBorder(ConnectDir dir, MGrid grid)
        {
            neighbors[(int)dir] = grid;
        }

        public void Clear()
        {
            neighbors = new MGrid[4];
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", IsConnected(ConnectDir.Right), IsConnected(ConnectDir.Up), IsConnected(ConnectDir.Left), IsConnected(ConnectDir.Down));
        }
    }

    public class MGrid : MonoBehaviour, INode
    {
        [SerializeField]
        private int2 position;
        [SerializeField]
        private GridType gType;
        public Connector connector;
        private SpriteRenderer render;

        public int2 Position
        {
            get { return position; }
        }

        public GridType GridType
        {
            get { return gType; }
            set { gType = value; }
        }

        public void Init(int2 pos)
        {
            render = GetComponent<SpriteRenderer>();
            connector = new Connector();
            position = pos;
            transform.position = new Vector3(pos.x, pos.y);
            Clear();
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

        public void Clear()
        {
            gType = GridType.Empty;
            connector.Clear();
            render.color = new Color(0, 0, 0, 0);
        }
        
        public static void SetBorder(MGrid a, MGrid b)
        {
            if(a == null || b == null)
                return;

            int2 offset = a.position - b.position;

            if (offset.AbsLen != 1)
                throw new Exception(a.position + " is not close to " + b.position);
            if (offset.x > 0)
            {
                b.connector.SetBorder(Connector.ConnectDir.Right, a);
                a.connector.SetBorder(Connector.ConnectDir.Left, b);
            }
            else if (offset.y > 0)
            {
                b.connector.SetBorder(Connector.ConnectDir.Up, a);
                a.connector.SetBorder(Connector.ConnectDir.Down, b);
            }
            else if (offset.x < 1)
            {
                b.connector.SetBorder(Connector.ConnectDir.Left, a);
                a.connector.SetBorder(Connector.ConnectDir.Right, b);
            }
            else if (offset.y < 0)
            {
                b.connector.SetBorder(Connector.ConnectDir.Down, a);
                a.connector.SetBorder(Connector.ConnectDir.Up, b);
            }
        }

        public override string ToString()
        {
            return (int)gType + " " + position;
        }

    }
}