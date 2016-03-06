using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class TileObject : MonoBehaviour
    {

        public Tile tile;

        SpriteRenderer renderer;
        
        public void InitializeGameobject()
        {
            renderer = gameObject.AddComponent<SpriteRenderer>();
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            renderer.sprite = tile.data.sprite;
        }
    }
}