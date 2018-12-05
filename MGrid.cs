using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyGrid
{
    public class MGrid : MonoBehaviour
    {
        public int2 position;
        public GridType gType;
        public Connector connector;
        private SpriteRenderer render;
        
        public void Init(int2 pos)
        {
            render = GetComponent<SpriteRenderer>();
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