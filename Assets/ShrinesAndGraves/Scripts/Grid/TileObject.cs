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
            var sgo = new GameObject("sprite");
            sgo.transform.SetParent(transform, false);
            renderer = sgo.AddComponent<SpriteRenderer>();
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            renderer.sprite = tile.data.sprite;
            var spr = renderer.sprite;
            if (spr != null)
            {
                renderer.transform.localPosition = -spr.Size() * 0.5f;
            }
        }
    }
}