using UnityEngine;

namespace TinyGrid
{
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
            return neighbors[(int) dir] != null;
        }

        public void SetBorder(ConnectDir dir, MGrid grid)
        {
            neighbors[(int) dir] = grid;
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

    public class TinyGenegrater : MonoBehaviour
    {
        
    }
}