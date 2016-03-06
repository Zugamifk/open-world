using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class Grid
    {
        Dictionary<Vector3i, Tile> tiles = new Dictionary<Vector3i,Tile>();

        public Tile this[Vector3i v]
        {
            get
            {
                return tiles[v];
            }
            set
            {
                tiles[v] = value;
            }
        }
    }
}