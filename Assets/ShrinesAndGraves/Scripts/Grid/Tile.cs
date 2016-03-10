using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class Tile : Entity
    {
        public TileData data;
        public Vector3i gridPosition;
        public bool collides
        {
            get
            {
                return data.collides;
            }
        }

        public override string name
        {
            get
            {
                return "Tile " + gridPosition.ToString();
            }
        }
    }
}